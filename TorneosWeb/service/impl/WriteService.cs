using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models;
using TorneosWeb.exception;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class WriteService : IWriteService
	{
		private IReadService readService;
		private ICacheService cacheService;
		private ILogger<WriteService> log;
		private IFileService tourneyReader;
		private ILigaWriter ligaWriter;
		private IProfitsExporter profitsExporter;
		private IPrizeService prizeService;

		private string connString;


		public WriteService(IReadService service, ICacheService cacheService, IConfiguration config, IProfitsExporter profitsExporter,
			IFileService tourneyReader, ILigaWriter ligaWriter, IPrizeService prizeService, ILogger<WriteService> logger)
		{
			readService = service;
			this.cacheService = cacheService;
			log = logger;
			connString = config.GetConnectionString( Properties.Resources.joserrasDb );
			this.tourneyReader = tourneyReader;
			this.ligaWriter = ligaWriter;
			this.profitsExporter = profitsExporter;
			this.prizeService = prizeService;
		}

		public void uploadTournament(List<IFormFile> files)
		{
			TorneoDTO torneo = tourneyReader.GetFormFileItems<TorneoDTO>( files.Find( t => t.FileName.Contains( "torneo" ) ) ).First();
			List<ResultadosDTO> resultados = tourneyReader.GetFormFileItems<ResultadosDTO>( files.Find( t => t.FileName.Contains( "resultados" ) ) ).ToList();
			List<KnockoutsDTO> kos = tourneyReader.GetFormFileItems<KnockoutsDTO>( files.Find( t => t.FileName.Contains( "knockouts" ) ) ).ToList();

			if(torneo == null || resultados == null || resultados.Count == 0 )
			{
				string msg = string.Empty;
				if(torneo == null )
				{
					msg = "No hay archivo cuyo nombre contenga la palabra 'torneo'";
				}
				else if(resultados == null )
				{
					msg = "No hay archivo cuyo nombre contenga la palabra 'resultados'";
				}
				else
				{
					msg = "Los resultados están vacíos";
				}
				log.LogError( msg );
				throw new JoserrasException( msg );
			}

			TorneoUnitOfWork uow = null;
			try
			{
				using( uow = new TorneoUnitOfWork( connString ) )
				{
					InsertarNuevosJugadores( resultados, uow );

					torneo.Id = InsertarTorneo( torneo, resultados, uow );

					InsertarResultados( torneo, resultados, uow );

					if(kos != null && kos.Count > 0 )
					{
						InsertarKos( torneo, resultados, kos, uow );
					}

					uow.Commit();
					cacheService.Clear();
				}
			}
			catch( JoserrasException je )
			{
				log.LogError( je, je.Message );
				try
				{
					uow.Rollback();
				}
				catch( Exception xe )
				{
					log.LogError( xe, xe.Message );
				}
				throw new JoserrasException( je.Message, je );
			}
			catch(Exception e )
			{
				log.LogError( e, e.Message );
				try
				{
					uow.Rollback();
				}
				catch( Exception xe )
				{
					log.LogError( xe, xe.Message );
				}
				throw new JoserrasException( e.Message, e );
			}

			if( torneo.Liga )
			{
				ligaWriter.AsociarTorneo( torneo.Id );
			}

			DayOfWeek dayOfWeek = torneo.Fecha.DayOfWeek;
			if(dayOfWeek == DayOfWeek.Sunday )
			{
				DateTime monday = torneo.Fecha.AddDays( -6 );
				List<Torneo> torneos = readService.GetAllTorneos().Where( t => monday <= t.FechaDate && t.FechaDate <= torneo.Fecha ).ToList();
				profitsExporter.ExportProfits( torneos );
			}
		}

		private void InsertarNuevosJugadores(List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			foreach( ResultadosDTO dto in resultados.Where( r => r.Nuevo ) )
			{
				AddPlayer( dto.Jugador, uow );
			}
		}

		private Guid InsertarTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			int rebuys = resultados.Sum( d => d.Rebuys );
			int bolsa = (torneo.PrecioBuyin * resultados.Count) + (torneo.PrecioRebuy * rebuys);
			torneo.Rebuys = rebuys;
			torneo.Bolsa = bolsa;

			Guid torneoId = Guid.Parse( uow.ExecuteScalar( Properties.Queries.InsertTorneo, torneo.Fecha.ToString( "yyyy-MM-dd" ),
					torneo.PrecioBuyin, torneo.PrecioRebuy, resultados.Count, rebuys, bolsa, torneo.Tipo.ToString(), torneo.PrecioBounty )
					.ToString() );

			return torneoId;
		}

		private void InsertarResultados(TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			string query = "insert into resultados (torneo_id, jugador_id, rebuys, posicion, podio, premio, burbuja, puntualidad) values('{0}', (select id from jugadores where nombre='{1}'), "
				+ "{2}, {3}, '{4}', {5}, '{6}', '{7}')";

			prizeService.SetPremiosTorneo( torneo, resultados );

			SetBurbuja( resultados );

			foreach( ResultadosDTO dto in resultados )
			{
				try
				{
					uow.ExecuteNonQuery( query, torneo.Id, dto.Jugador, dto.Rebuys, dto.Posicion,
							dto.Premio > 0 ? true.ToString() : false.ToString(), dto.Premio, dto.Burbuja.ToString(), dto.Puntualidad.ToString() );
				}
				catch(SqlException sqle )
				{
					if(sqle.Number == 515 )
					{
						string msg = string.Format( "El Jugador '{0}' no existe. No se agregó el torneo.", dto.Jugador );
						log.LogError( sqle, sqle.Message );
						throw new JoserrasException( msg, sqle );
					}
					else
					{
						string msg = string.Format( "No se pudo agregar el resultado del Jugador '{0}'. No se agregó el torneo.", dto.Jugador );
						log.LogError( sqle, sqle.Message );
						throw new JoserrasException( msg, sqle );
					}
				}
				catch( Exception e )
				{
					string msg = string.Format( "No se pudo agregar el resultado del Jugador '{0}'. No se agregó el torneo.", dto.Jugador );
					log.LogError( e, e.Message );
					throw new JoserrasException( msg, e );
				}
			}
		}

		private void SetBurbuja(List<ResultadosDTO> resultados)
		{
			int bubblePosition = resultados.Where( r => r.Premio > 0 ).Max( r => r.Posicion ) + 1;
			resultados.First( r => r.Posicion == bubblePosition ).Burbuja = true;
		}

		private void InsertarKos(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow)
		{
			string query = "insert into knockouts (torneo_id, jugador_id, eliminado_id, eliminaciones) values('{0}', "
				+ "(select id from jugadores where nombre = '{1}'), (select id from jugadores where nombre = '{2}'), {3})";

			foreach(KnockoutsDTO dto in kos )
			{
				try
				{
					uow.ExecuteNonQuery( query, torneo.Id, dto.Jugador, dto.Eliminado, dto.Eliminaciones );
				}
				catch( Exception e )
				{
					string msg = string.Format( "Error al insertar el KO de '{0}' a '{1}' en la tabla de Knockouts. No se agregó el torneo.", dto.Jugador, dto.Eliminado );
					log.LogError( e, msg );
					throw new JoserrasException( msg );
				}
			}

			ResultadosDTO firstPlace = resultados.First( d => d.Posicion == 1 );

			// Insertar kos y bounties
			query = @"update resultados set premio_bounties = {0}, kos = {1} where torneo_id = '{2}' and jugador_id = (select id from jugadores where nombre = '{3}')";
			IEnumerable<Tuple<string, int>> tuples =
				kos.GroupBy( k => k.Jugador ).Select( s => new Tuple<string, int>( s.First().Jugador, s.Sum( c => c.Eliminaciones ) ) );
			foreach( Tuple<string, int> t in tuples )
			{
				int bountyPrice = t.Item2 * torneo.PrecioBounty;
				if(torneo.Tipo == TournamentType.BOUNTY && t.Item1 == firstPlace.Jugador )
				{
					bountyPrice += torneo.PrecioBounty;
				}
				string q = string.Format( query, bountyPrice, t.Item2, torneo.Id, t.Item1 );
				try
				{
					resultados.Where( r => r.Jugador == t.Item1 ).First().Kos = t.Item2;
					uow.ExecuteNonQuery( query, bountyPrice, t.Item2, torneo.Id, t.Item1 );
				}
				catch( Exception e )
				{
					string msg = string.Format( "Error al actualizar la tabla de Resultados con los kos de: '{0}'. No se agregó el torneo", t.Item1 );
					log.LogError( e, msg );
					throw new JoserrasException( msg, e );
				}
			}
		}

		private void AddPlayer(string nombre, TorneoUnitOfWork uow)
		{
			string query = "insert into jugadores (nombre) values ('{0}')";

			try
			{
				uow.ExecuteNonQuery( query, nombre );
				uow.Commit();
				cacheService.Clear();
			}
			catch( SqlException sqle )
			{
				string msg = "";
				if( sqle.Number == 2627 )
				{
					msg = string.Format( "El jugador '{0}' ya existe", nombre );
					log.LogError( sqle, msg );
					throw new JoserrasException( msg, sqle );
				}
				else
				{
					msg = string.Format( "No se pudo agregar el jugador: '{0}'", nombre );
					log.LogError( sqle, msg );
					throw new JoserrasException( msg, sqle );
				}
			}
			catch( Exception e )
			{
				string msg = string.Format( "No se pudo agregar el jugador: '{0}'", nombre );
				log.LogError( e, msg );
				throw new JoserrasException( msg, e );
			}
		}

		public void AddPlayer(string nombre)
		{
			using(TorneoUnitOfWork uow = new TorneoUnitOfWork( connString ) )
			{
				AddPlayer( nombre, uow );
			}
		}

	}

}