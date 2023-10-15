using Humanizer;
using Joserras.Commons.Db;
using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using Joserras.Commons.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.Properties;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class WriteService : IWriteService
	{
		private ICacheService cacheService;
		private ILogger<WriteService> log;
		private IPrizeService prizeService;

		private readonly string connString;


		public WriteService(ICacheService cacheService, IConfiguration config, IPrizeService prizeService,
			ILogger<WriteService> logger)
		{
			this.cacheService = cacheService;
			log = logger;
			connString = config.GetConnectionString( Properties.Resources.joserrasDb );
			this.prizeService = prizeService;
		}

		public Guid UploadTournament(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
		{
			if( torneo == null || resultados == null || resultados.Count == 0 )
			{
				string msg;
				if( torneo == null )
				{
					msg = "No hay archivo cuyo nombre contenga la palabra 'torneo'";
				}
				else if( resultados == null )
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

			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( connString ) )
			{
				try
				{
					InsertarNuevosJugadores( resultados, uow );

					torneo.Id = InsertarTorneo( torneo, resultados, uow );

					InsertarResultados( torneo, resultados, uow );

					if( kos != null && kos.Count > 0 )
					{
						InsertarKos( torneo, resultados, kos, uow );
					}

					uow.Commit();
					cacheService.Clear();
				}
				catch( JoserrasException je )
				{
					log.LogError( je, je.Message );
					uow.Rollback();
					throw;
				}
				catch( Exception e )
				{
					log.LogError( e, e.Message );
					uow.Rollback();
					throw new JoserrasException( e.Message, e );
				}
			}

			log.LogDebug( "Torneo con id: '{0}' ha sido agregado exitosamente", torneo.Id );
			return torneo.Id;
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
			if(torneo.PrecioRebuy == 0  && torneo.Tipo != TournamentType.FREEZEOUT)
			{
				torneo.PrecioRebuy = torneo.PrecioBuyin;
			}

			torneo.Entradas = resultados.Count;
			torneo.Rebuys = resultados.Sum( d => d.Rebuys );
			torneo.Bolsa = prizeService.GetBolsaTorneo( torneo);

			torneo.Premiacion = prizeService.GetPremiacionString( torneo, resultados );

			Guid torneoId = Guid.Parse( uow.ExecuteScalar( Queries.InsertTorneo, torneo.Fecha.ToString( "yyyy-MM-dd" ),
					torneo.PrecioBuyin, torneo.PrecioRebuy, torneo.Entradas, torneo.Rebuys, torneo.Bolsa.Total,
					torneo.Tipo.ToString(), torneo.PrecioBounty, torneo.Premiacion )
					.ToString() );

			return torneoId;
		}

		private void InsertarResultados(TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			string query = "insert into resultados (torneo_id, jugador_id, rebuys, posicion, podio, premio, burbuja, puntualidad) values('{0}', (select id from jugadores where nombre='{1}'), "
				+ "{2}, {3}, '{4}', {5}, '{6}', '{7}')";

			IDictionary<int, string> prizes = prizeService.GetPremios( torneo, resultados );

			foreach(int key in prizes.Keys)
			{
				resultados.Single( r => r.Posicion == key ).Premio = prizes[key];
			}

			foreach(ResultadosDTO res in resultados)
			{
				log.LogDebug( "Posición: {0} - Premio: {1}", res.Posicion, res.Premio );
			}

			if(resultados.Sum( r => decimal.Parse( r.Premio ) ) != torneo.Bolsa.Premios)
			{
				string msg = string.Format( "Los premios otorgados suman {0} pero la Bolsa es igual a {1}",
					resultados.Sum( r => decimal.Parse( r.Premio ) ).ToString( Constants.CURRENCY_FORMAT ),
					torneo.Bolsa.Premios.ToString( Constants.CURRENCY_FORMAT ) );

				throw new JoserrasException( msg );
			}

			SetBurbuja( resultados );

			foreach( ResultadosDTO dto in resultados )
			{
				try
				{
					uow.ExecuteNonQuery( query, torneo.Id, dto.Jugador, dto.Rebuys, dto.Posicion,
							dto.Premio.ToDecimal() > 0 ? true.ToString() : false.ToString(), dto.Premio.ToDecimal(), dto.Burbuja.ToString(), dto.Puntualidad.ToString() );
				}
				catch(SqlException sqle )
				{
					if(sqle.Number == 515 )
					{
						string msg = string.Format( "El Jugador '{0}' no existe. No se agregó el torneo.", dto.Jugador );
						throw new JoserrasException( msg, sqle );
					}
					else
					{
						string msg = string.Format( "No se pudo agregar el resultado del Jugador '{0}'. No se agregó el torneo.", dto.Jugador );
						throw new JoserrasException( msg, sqle );
					}
				}
				catch( Exception e )
				{
					string msg = string.Format( "No se pudo agregar el resultado del Jugador '{0}'. No se agregó el torneo.", dto.Jugador );
					throw new JoserrasException( msg, e );
				}
			}
		}

		private void SetBurbuja(List<ResultadosDTO> resultados)
		{
			int bubblePosition = resultados.Where( r => r.Premio.ToDecimal() > 0 ).Max( r => r.Posicion ) + 1;
			log.LogDebug( "Posición de la Burbuja: {0}", bubblePosition );
			resultados.First( r => r.Posicion == bubblePosition ).Burbuja = true;
		}

		private void InsertarKos(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow)
		{
			string query;

			if( !string.IsNullOrEmpty( kos.First().Eliminado ) )
			{
				query = @"insert into knockouts (torneo_id, jugador_id, eliminado_id, eliminaciones, mano_url) values(@torneoId, "
						+ @"(select id from jugadores where nombre = @jugador), (select id from jugadores where nombre = @eliminado), @kos, @mano)";

				foreach( KnockoutsDTO dto in kos )
				{
					try
					{
						IDictionary<string, object> parameters = new Dictionary<string, object>
					{
						{ "@torneoId", torneo.Id },
						{ "@jugador", dto.Jugador },
						{ "@eliminado", dto.Eliminado },
						{ "@kos", dto.Eliminaciones },
						{ "@mano", dto.Mano }
					};

						uow.ExecuteNonQuery( query, parameters );
					}
					catch( Exception e )
					{
						string msg = string.Format( "Error al insertar el KO de '{0}' a '{1}' en la tabla de Knockouts. No se agregó el torneo.", dto.Jugador, dto.Eliminado );
						throw new JoserrasException( msg, e );
					}
				}
			}

			query = SetKos( torneo, resultados, kos, uow );
		}

		private static string SetKos(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow)
		{
			string query;
			ResultadosDTO firstPlace = resultados.First( d => d.Posicion == 1 );

			// Insertar kos y bounties
			query = @"update resultados set premio_bounties = {0}, kos = {1} where torneo_id = '{2}' and jugador_id = (select id from jugadores where nombre = '{3}')";
			IEnumerable<Tuple<string, decimal>> tuples =
				kos.GroupBy( k => k.Jugador ).Select( s => new Tuple<string, decimal>( s.First().Jugador, s.Sum( j => j.Eliminaciones ) ) );
			foreach( Tuple<string, decimal> t in tuples )
			{
				decimal bountyPrice = t.Item2 * torneo.PrecioBounty;
				if( torneo.Tipo == TournamentType.BOUNTY && t.Item1 == firstPlace.Jugador )
				{
					bountyPrice += torneo.PrecioBounty;
				}
				string q = string.Format( query, bountyPrice, t.Item2, torneo.Id, t.Item1 );
				try
				{
					resultados.First( r => r.Jugador == t.Item1 ).Kos = t.Item2;
					_ = uow.ExecuteNonQuery( query, bountyPrice, t.Item2, torneo.Id, t.Item1 );
				}
				catch( Exception e )
				{
					string msg = string.Format( "Error al actualizar la tabla de Resultados con los kos de: '{0}'. No se agregó el torneo", t.Item1 );
					throw new JoserrasException( msg, e );
				}
			}

			return query;
		}

		private void AddPlayer(string nombre, TorneoUnitOfWork uow)
		{
			string query = "insert into jugadores (nombre) values ('{0}')";

			try
			{
				uow.ExecuteNonQuery( query, nombre );
			}
			catch( SqlException sqle )
			{
				string msg = "";
				if( sqle.Number == 2627 )
				{
					msg = string.Format( "El jugador '{0}' ya existe", nombre );
					throw new JoserrasException( msg, sqle );
				}
				else
				{
					msg = string.Format( "No se pudo agregar el jugador: '{0}'", nombre );
					throw new JoserrasException( msg, sqle );
				}
			}
			catch( Exception e )
			{
				string msg = string.Format( "No se pudo agregar el jugador: '{0}'", nombre );
				throw new JoserrasException( msg, e );
			}
		}

		public void AddPlayer(string nombre)
		{
			using(TorneoUnitOfWork uow = new TorneoUnitOfWork( connString ) )
			{
				AddPlayer( nombre, uow );
				uow.Commit();
				cacheService.Clear();
			}
		}

		public JoserrasActionResult DeleteTorneo(Guid torneoId)
		{
			JoserrasActionResult result = new();

			using( TorneoUnitOfWork uow = new( connString ) )
			{
				try
				{
					string query;
					IDictionary<string, object> parameters = new Dictionary<string, object>
					{
						{ "@torneoId", torneoId }
					};
					
					query = @"delete from torneos_liga where torneo_id = @torneoId";
					uow.ExecuteNonQuery( query, parameters );

					query = @"delete from knockouts where torneo_id = @torneoId";
					uow.ExecuteNonQuery( query, parameters );

					query = @"delete from resultados where torneo_id = @torneoId";
					uow.ExecuteNonQuery( query, parameters );

					query = @"delete from torneos where id = @torneoId";
					uow.ExecuteNonQuery( query, parameters );

					uow.Commit();
					cacheService.Clear();
					result.ActionSucceeded = true;
					log.LogDebug( "El torneo con id '{0}' fue eliminado correctamente", torneoId );
				}
				catch(Exception e )
				{
					uow.Rollback();
					log.LogWarning( e, e.Message );
					result.Description = string.Format( "No se pudo eliminar el torneo con id: {0}", torneoId );
				}
			}
			
			return result;
		}

	}

}