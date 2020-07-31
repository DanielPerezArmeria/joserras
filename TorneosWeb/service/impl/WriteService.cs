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

namespace TorneosWeb.service.impl
{
	public class WriteService : IWriteService
	{
		private IReadService readService;
		private ICacheService cacheService;
		private ILogger<WriteService> log;
		private ITournamentReader tourneyReader;

		private string connString;

		public WriteService(IReadService service, ICacheService cacheService, IConfiguration config, ITournamentReader tourneyReader, ILogger<WriteService> logger)
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
			List<EliminacionesDTO> kos = tourneyReader.GetItems<EliminacionesDTO>( files.Find( t => t.FileName.Contains( "knockouts" ) ) ).ToList();

			SqlTransaction transaction = null;
			try
			{
				using( SqlConnection connection = new SqlConnection( connString ) )
				{
					connection.Open();
					transaction = connection.BeginTransaction();

					insertarNuevosJugadores( resultados, connection, transaction );

					torneo.Id = insertarTorneo( torneo, resultados, connection, transaction );

					insertarDetallesTorneo( torneo.Id, resultados, connection, transaction );

					if(kos != null && kos.Count > 0 )
					{
						insertarKos( torneo, resultados, kos, connection, transaction );
					}

					transaction.Commit();
				}
				cacheService.Clear();
			}
			catch(Exception e )
			{
				log.LogError( e, e.Message );
				try
				{
					transaction.Rollback();
				}
				catch( Exception xe )
				{
					log.LogError( xe, xe.Message );
					throw;
				}
				throw new JoserrasException( e );
			}
		}

		private void insertarNuevosJugadores(List<ResultadosDTO> resultados, SqlConnection conn, SqlTransaction tx)
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
						new SqlCommand( string.Format( query, dto.Jugador ), conn, tx ).ExecuteNonQuery();
					}
					catch( Exception e )
					{
						log.LogError( e, e.Message );
						throw;
					}
				}
			}
		}

		private Guid insertarTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados, SqlConnection conn, SqlTransaction tx)
		{
			int rebuys = resultados.Sum( d => d.Rebuys );
			int bolsa = (torneo.PrecioBuyin * resultados.Count) + (torneo.PrecioRebuy * rebuys);
			string query = string.Format( Properties.Queries.InsertTorneo, torneo.Fecha.ToString( "yyyy-MM-dd" ),
				torneo.PrecioBuyin, torneo.PrecioRebuy, resultados.Count, rebuys, bolsa, torneo.Tipo.ToString(), torneo.PrecioBounty );

			Guid torneoId = Guid.Parse( new SqlCommand( query, conn, tx ).ExecuteScalar().ToString() );

			return torneoId;
		}

		private void insertarDetallesTorneo(Guid torneoId, List<ResultadosDTO> resultados, SqlConnection conn, SqlTransaction tx)
		{
			string query = "insert into resultados (torneo_id, jugador_id, rebuys, posicion, podio, premio, burbuja) values('{0}', (select id from jugadores where nombre='{1}'), "
				+ "{2}, {3}, '{4}', {5}, '{6}')";

			foreach( ResultadosDTO dto in resultados )
			{
				string q = string.Format( query, torneoId, dto.Jugador, dto.Rebuys, dto.Posicion,
					dto.Premio > 0 ? true.ToString() : false.ToString(), dto.Premio, dto.Burbuja.ToString() );
				try
				{
					new SqlCommand( q, conn, tx ).ExecuteNonQuery();
				}
				catch( Exception e )
				{
					log.LogError( e, e.Message );
					throw;
				}
			}
		}

		private void insertarKos(TorneoDTO torneo, List<ResultadosDTO> resultados, List<EliminacionesDTO> kos, SqlConnection conn, SqlTransaction tx)
		{
			string query = "insert into eliminaciones (torneo_id, jugador_id, eliminado_id, eliminaciones) values('{0}', "
				+ "(select id from jugadores where nombre = '{1}'), (select id from jugadores where nombre = '{2}'), {3})";

			foreach(EliminacionesDTO dto in kos )
			{
				string q = string.Format( query, torneo.Id, dto.Jugador, dto.Eliminado, dto.Eliminaciones );
				try
				{
					new SqlCommand( q, conn, tx ).ExecuteNonQuery();
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
					new SqlCommand( q, conn, tx ).ExecuteNonQuery();
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