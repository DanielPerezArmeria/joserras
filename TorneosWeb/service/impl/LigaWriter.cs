using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.dto.ligas;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.exception;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.service.impl
{
	public class LigaWriter : ILigaWriter
	{
		private ILogger<LigaWriter> log;

		private IReadService readService;
		private IFileService ligaDataReader;
		private ILigaReader ligaReader;
		private IConfiguration config;
		private ICacheService cacheService;
		private IStorageDao storageDao;

		public LigaWriter(IReadService readService, IFileService dataReader, ILigaReader ligaReader, IStorageDao storageDao,
				IConfiguration config, ICacheService cacheService, ILogger<LigaWriter> log)
		{
			this.readService = readService;
			ligaDataReader = dataReader;
			this.ligaReader = ligaReader;
			this.config = config;
			this.log = log;
			this.cacheService = cacheService;
			this.storageDao = storageDao;
		}

		public void AgregarNuevaLiga(IFormFile file)
		{
			LigaDTO liga = ligaDataReader.GetFormFileItems<LigaDTO>( file ).First();
			using( TorneoUnitOfWork uow = new( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				string query = @"insert into ligas (nombre, abierta, puntaje, fee, premiacion, desempate) values ('{0}', {1}, '{2}', {3}, '{4}', '{5}')";
				uow.ExecuteNonQuery( query, liga.Nombre, 1, liga.Puntaje, liga.Fee, liga.Premiacion, liga.Desempate );

				uow.Commit();
			}

			cacheService.Clear();
		}

		public int AsociarTorneo(Guid torneoId)
		{
			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				try
				{
					int asociar = AsociarTorneo( torneoId, uow );
					uow.Commit();
					cacheService.Clear();
					return asociar;
				}
				catch( Exception e )
				{
					log.LogError( e, e.Message );
					uow.Rollback();
					string msg = string.Format( "No se pudo asociar el torneo con id: {0}", torneoId );
					throw new JoserrasException( msg, e );
				}
			}
		}

		private void SaveStandings(Liga liga, Guid torneoId)
		{
			Torneo torneo = readService.GetAllTorneos().Single( t => t.Id.Equals( torneoId ) );
			List<Standing> torneoStandings = ligaReader.GetStandings( liga, torneo );
			List<Standing> reglaStandings = new();
			List<Standing> ligaStandings = new();
			foreach (Standing s in torneoStandings)
			{
				SortedDictionary<PointRuleType, FDecimal> points = new( s.Puntos.Where( p => p.Key.GetScope().Equals( RuleScope.TORNEO ) ).ToDictionary( x => x.Key, x => x.Value ) );
				reglaStandings.Add( new() { Jugador = s.Jugador, JugadorId = s.JugadorId, Puntos = points } );

				points = new( s.Puntos.Where( p => p.Key.GetScope().Equals( RuleScope.LIGA ) ).ToDictionary( x => x.Key, x => x.Value ) );
				ligaStandings.Add( new() { Jugador = s.Jugador, JugadorId = s.JugadorId, Puntos = points } );
			}
			
			storageDao.SaveTorneoStandings( AzureTables.PUNTOS_TORNEO_TABLE, torneoId, torneoStandings );
			storageDao.SaveTorneoStandings( AzureTables.PUNTOS_LIGA_TABLE, torneoId, ligaStandings );
		}

		private int AsociarTorneo(Guid torneoId, TorneoUnitOfWork uow)
		{
			if( ligaReader.GetCurrentLiga() == null )
			{
				log.LogWarning( "No hay una Liga abierta. No se puede asociar el torneo." );
				return 0;
			}

			int rowsAffected;

			Liga liga = ligaReader.GetCurrentLiga();
			log.LogDebug( "Asociando torneo con id '{0}' en Liga '{1}'", torneoId, liga.Nombre );
			string query = "insert into torneos_liga (liga_id, torneo_id) values ('{0}', '{1}')";
			try
			{
				rowsAffected = uow.ExecuteNonQuery( query, liga.Id, torneoId );
				SaveStandings( liga, torneoId );
			}
			catch( Exception )
			{
				throw;
			}

			return rowsAffected;
		}

		public int AsociarTorneoEnFecha(DateTime date)
		{
			Torneo torneo = readService.FindTorneoByFecha( date );
			if(torneo == null )
			{
				log.LogError( "No existe torneo con fecha: {0}", date );
				return 0;
			}

			return AsociarTorneo( torneo.Id );
		}

		public void CerrarLiga()
		{
			Liga liga = ligaReader.GetCurrentLiga();
			List<Standing> standings = ligaReader.GetStandings( liga );
			SetPremiosLiga( liga, standings );
			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				try
				{
					string query = "insert into puntos_torneo_liga values ('{0}', '{1}', {2}, {3})";
					foreach( Standing standing in standings )
					{
						string q = string.Format( query, liga.Id, standing.JugadorId, standing.Total, standing.PremioLigaNumber );
						try
						{
							log.LogInformation( "Guardando: {0}", q );
							uow.ExecuteNonQuery( q );
						}
						catch( Exception e )
						{
							string msg = string.Format( "Error al guardar el Standing: {0}", q );
							throw new JoserrasException( msg, e );
						}
					}

					string updateLigaQuery = "update ligas set Abierta = {0}, fecha_cierre = '{1}' where id = '{2}'";
					try
					{
						uow.ExecuteNonQuery( updateLigaQuery, 0, liga.Torneos.First().FechaDate.ToString( "yyyy-MM-dd" ), liga.Id );
					}
					catch( Exception e )
					{
						throw new JoserrasException( "Error al cerrar la liga", e );
					}

					uow.Commit();
					cacheService.Clear();
				}
				catch( Exception e)
				{
					log.LogError( e, e.Message );
					uow.Rollback();
					throw;
				}
			}
		}

		private void SetPremiosLiga(Liga liga, List<Standing> standings)
		{
			string[] prizesString = liga.Premiacion.Split( "-" );
			int placesAwarded = prizesString.Length;
			decimal totalToSplit = liga.GetAcumulado();
			for(int i = placesAwarded - 1; i >= 0; i-- )
			{
				Standing standing = standings.ElementAt( i );
				decimal factor = decimal.Parse( prizesString[ i ].Replace( "%", "" ) ) / 100;
				standing.PremioLigaNumber = totalToSplit * factor;
			}
		}

	}

}