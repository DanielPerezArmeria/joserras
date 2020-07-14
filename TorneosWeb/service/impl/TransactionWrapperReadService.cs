using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.exception;

namespace TorneosWeb.service.impl
{
	public class TransactionWrapperReadService : IReadService
	{
		private IReadService wrapped;
		private ICacheService cacheService;
		private ILogger<TransactionWrapperReadService> log;

		public TransactionWrapperReadService(IReadService readService, ICacheService cacheService, ILogger<TransactionWrapperReadService> logger)
		{
			wrapped = readService;
			this.cacheService = cacheService;
			log = logger;
		}

		public List<Jugador> GetAllJugadores()
		{
			if( cacheService.GetAllJugadores.Count == 0 )
			{
				try
				{
					cacheService.GetAllJugadores.AddRange( wrapped.GetAllJugadores() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetAllJugadores;
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			if( cacheService.GetAllKnockouts.Count == 0 )
			{
				try
				{
					foreach( KeyValuePair<string, Dictionary<string, Knockouts>> item in wrapped.GetAllKnockouts() )
					{
						cacheService.GetAllKnockouts.Add( item.Key, item.Value );
					}
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetAllKnockouts;
		}

		public List<Torneo> GetAllTorneos()
		{
			if( cacheService.GetAllTorneos.Count == 0 )
			{
				try
				{
					cacheService.GetAllTorneos.AddRange( wrapped.GetAllTorneos() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetAllTorneos;
		}

		public DetalleTorneo GetDetalleTorneo(Guid id)
		{
			if( !cacheService.GetDetalleTorneo.ContainsKey(id) )
			{
				try
				{
					cacheService.GetDetalleTorneo.Add( id, wrapped.GetDetalleTorneo( id ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetDetalleTorneo[id];
		}

		public DetalleJugador GetDetalleJugador(Guid id)
		{
			if( !cacheService.GetDetalleJugadorById.ContainsKey( id ) )
			{
				try
				{
					cacheService.GetDetalleJugadorById.Add( id, wrapped.GetDetalleJugador( id ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetDetalleJugadorById[ id ];
		}

		public DetalleJugador GetDetalleJugador(string nombre)
		{
			throw new NotImplementedException();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockoutsByTournamentId(Guid torneoId)
		{
			if( !cacheService.GetKnockoutsByTournament.ContainsKey( torneoId ) )
			{
				try
				{
					cacheService.GetKnockoutsByTournament.Add( torneoId, wrapped.GetKnockoutsByTournamentId( torneoId ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetKnockoutsByTournament[ torneoId ];
		}

		public List<Knockouts> GetKnockoutsByPlayer(Guid playerId)
		{
			if( !cacheService.GetKnockoutsByPlayer.ContainsKey( playerId ) )
			{
				try
				{
					cacheService.GetKnockoutsByPlayer.Add( playerId, wrapped.GetKnockoutsByPlayer( playerId ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetKnockoutsByPlayer[ playerId ];
		}

		public List<DetalleJugador> GetDetalleJugador()
		{
			if( cacheService.GetDetalleJugador.Count == 0 )
			{
				try
				{
					cacheService.GetDetalleJugador.AddRange( wrapped.GetDetalleJugador() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetDetalleJugador;
		}

		public Estadisticas GetStats()
		{
			if( cacheService.GetStats == null )
			{
				try
				{
					cacheService.GetStats = wrapped.GetStats();
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetStats;
		}
	}

}