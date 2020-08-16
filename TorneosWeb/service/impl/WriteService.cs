using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
		private ITorneoDataReader tourneyReader;

		private string connString;

		public WriteService(IReadService service, ICacheService cacheService, IConfiguration config,
			ITorneoDataReader tourneyReader, ILogger<WriteService> logger)
		{
			readService = service;
			this.cacheService = cacheService;
			log = logger;
			connString = config.GetConnectionString( Properties.Resources.joserrasDb );
			this.tourneyReader = tourneyReader;
		}

		public void uploadTournament(List<IFormFile> files)
		{
			TorneoDTO torneo = tourneyReader.GetItems<TorneoDTO>( files.Find( t => t.FileName.Contains( "torneo" ) ) ).First();
			List<ResultadosDTO> resultados = tourneyReader.GetItems<ResultadosDTO>( files.Find( t => t.FileName.Contains( "resultados" ) ) ).ToList();
			List<KnockoutsDTO> kos = tourneyReader.GetItems<KnockoutsDTO>( files.Find( t => t.FileName.Contains( "knockouts" ) ) ).ToList();

			TorneoUnitOfWork uow = null;
			try
			{
				using( uow = new TorneoUnitOfWork( connString ) )
				{
					InsertarNuevosJugadores( resultados, uow );

					torneo.Id = InsertarTorneo( torneo, resultados, uow );

					InsertarResultados( torneo.Id, resultados, uow );

					if(kos != null && kos.Count > 0 )
					{
						InsertarKos( torneo, resultados, kos, uow );
					}

					uow.Commit();
				}
				cacheService.Clear();
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
				throw new JoserrasException( e );
			}
		}

		private void InsertarNuevosJugadores(List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			List<Jugador> jugadores = readService.GetAllJugadores();
			List<ResultadosDTO> newPlayers = new List<ResultadosDTO>();
			string query = "insert into jugadores (nombre) values ('{0}');";
			foreach( ResultadosDTO dto in resultados )
			{
				if( jugadores.Find( d => d.Nombre == dto.Jugador ) == null )
				{
					try
					{
						uow.ExecuteNonQuery( query, dto.Jugador );
					}
					catch( Exception e )
					{
						log.LogError( e, e.Message );
						throw;
					}
				}
			}
		}

		private Guid InsertarTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			int rebuys = resultados.Sum( d => d.Rebuys );
			int bolsa = (torneo.PrecioBuyin * resultados.Count) + (torneo.PrecioRebuy * rebuys);

			Guid torneoId = Guid.Parse( uow.ExecuteScalar( Properties.Queries.InsertTorneo, torneo.Fecha.ToString( "yyyy-MM-dd" ),
					torneo.PrecioBuyin, torneo.PrecioRebuy, resultados.Count, rebuys, bolsa, torneo.Tipo.ToString(), torneo.PrecioBounty )
					.ToString() );

			return torneoId;
		}

		private void InsertarResultados(Guid torneoId, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			string query = "insert into resultados (torneo_id, jugador_id, rebuys, posicion, podio, premio, burbuja, puntualidad) values('{0}', (select id from jugadores where nombre='{1}'), "
				+ "{2}, {3}, '{4}', {5}, '{6}', '{7}')";

			foreach( ResultadosDTO dto in resultados )
			{
				try
				{
					uow.ExecuteNonQuery( query, torneoId, dto.Jugador, dto.Rebuys, dto.Posicion,
							dto.Premio > 0 ? true.ToString() : false.ToString(), dto.Premio, dto.Burbuja.ToString(), dto.Puntualidad.ToString() );
				}
				catch( Exception e )
				{
					log.LogError( e, e.Message );
					throw;
				}
			}
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
					log.LogError( e, e.Message );
					throw;
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
					log.LogError( e, e.Message );
					throw;
				}
			}
		}

	}

}