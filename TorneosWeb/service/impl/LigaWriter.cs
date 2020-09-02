using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.dto.ligas;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;

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

		public LigaWriter(IReadService readService, IFileService dataReader, ILigaReader ligaReader,
				IConfiguration config, ICacheService cacheService, ILogger<LigaWriter> log)
		{
			this.readService = readService;
			ligaDataReader = dataReader;
			this.ligaReader = ligaReader;
			this.config = config;
			this.log = log;
			this.cacheService = cacheService;
		}

		public void AgregarNuevaLiga(IFormFile file)
		{
			LigaDTO liga = ligaDataReader.GetFormFileItems<LigaDTO>( file ).First();
			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				string query = @"insert into ligas (nombre, abierta, puntaje, fee, premiacion) values ('{0}', {1}, '{2}', {3}, '{4}')";
				uow.ExecuteNonQuery( query, liga.Nombre, 1, liga.Puntaje, liga.Fee, liga.Premiacion );

				uow.Commit();
			}

			cacheService.Clear();
		}

		public int AsociarTorneo(Guid torneoId)
		{
			if( ligaReader.GetCurrentLiga() == null )
			{
				log.LogWarning( "No hay una Liga abierta. No se puede asociar el torneo." );
				return 0;
			}

			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				int rowsAffected = 0;

				Liga liga = ligaReader.GetCurrentLiga();
				log.LogDebug( "Asociando torneo con id'{0}' en Liga '{1}'", torneoId, liga.Nombre );
				string query = "insert into torneos_liga (liga_id, torneo_id) values ('{0}', '{1}')";
				try
				{
					rowsAffected = uow.ExecuteNonQuery( query, liga.Id, torneoId );
				}
				catch( Exception )
				{
					throw;
				}

				uow.Commit();
				cacheService.Clear();

				return rowsAffected; 
			}
		}

		public int AsociarTorneoEnFecha(DateTime date)
		{
			Torneo torneo = readService.FindTorneoByFecha( date );
			if(torneo == null )
			{
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
				string query = "insert into puntos_torneo_liga values ('{0}', '{1}', {2}, {3})";
				foreach(Standing standing in standings )
				{
					uow.ExecuteNonQuery( query, liga.Id, standing.JugadorId, standing.Total, standing.PremioNumber );
				}

				string updateLigaQuery = "update ligas set Abierta = {0}, fecha_cierre = '{1}' where id = '{2}'";
				uow.ExecuteNonQuery( updateLigaQuery, 0, liga.Torneos.First().FechaDate.ToString( "yyyy-MM-dd" ), liga.Id );

				uow.Commit();
				cacheService.Clear();
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
				standing.PremioNumber = totalToSplit * factor;
			}
		}

	}

}