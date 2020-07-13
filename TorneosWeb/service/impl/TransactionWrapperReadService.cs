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
			if( !cacheService.ContainsKey( "GetAllJugadores" ) )
			{
				try
				{
					cacheService.Add( "GetAllJugadores", wrapped.GetAllJugadores() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetAllJugadores" ] as List<Jugador>;
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			if( !cacheService.ContainsKey( "GetAllKnockouts" ) )
			{
				try
				{
					cacheService.Add( "GetAllKnockouts", wrapped.GetAllKnockouts() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetAllKnockouts" ] as SortedList<string, Dictionary<string, Knockouts>>;
		}

		public List<Torneo> GetAllTorneos()
		{
			if( !cacheService.ContainsKey( "GetAllTorneos" ) )
			{
				try
				{
					cacheService.Add( "GetAllTorneos", wrapped.GetAllTorneos() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetAllTorneos" ] as List<Torneo>;
		}

		public DetalleTorneo GetDetalleTorneo(Guid id)
		{
			if( !cacheService.ContainsKey( "GetDetalleTorneo" ) )
			{
				try
				{
					cacheService.Add( "GetDetalleTorneo", wrapped.GetDetalleTorneo( id ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetDetalleTorneo" ] as DetalleTorneo;
		}

		public DetalleJugador GetDetalleJugador(Guid id)
		{
			if( !cacheService.ContainsKey( "GetDetalleJugadorById" ) )
			{
				try
				{
					cacheService.Add( "GetDetalleJugadorById", wrapped.GetDetalleJugador( id ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetDetalleJugadorById" ] as DetalleJugador;
		}

		public DetalleJugador GetDetalleJugador(string nombre)
		{
			throw new NotImplementedException();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockouts(Guid torneoId)
		{
			if( !cacheService.ContainsKey( "GetKnockouts" ) )
			{
				try
				{
					cacheService.Add( "GetKnockouts", wrapped.GetKnockouts( torneoId ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetKnockouts" ] as SortedList<string, Dictionary<string, Knockouts>>;
		}

		public List<Knockouts> GetKnockoutsByPlayer(Guid playerId)
		{
			if( !cacheService.ContainsKey( "GetKnockoutsByPlayer" ) )
			{
				try
				{
					cacheService.Add( "GetKnockoutsByPlayer", wrapped.GetKnockoutsByPlayer( playerId ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetKnockoutsByPlayer" ] as List<Knockouts>;
		}

		public List<DetalleJugador> GetDetalleJugador()
		{
			if( !cacheService.ContainsKey( "GetDetalleJugador" ) )
			{
				try
				{
					cacheService.Add( "GetDetalleJugador", wrapped.GetDetalleJugador() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetDetalleJugador" ] as List<DetalleJugador>;
		}

		public Estadisticas GetStats()
		{
			if( !cacheService.ContainsKey( "GetStats" ) )
			{
				try
				{
					cacheService.Add( "GetStats", wrapped.GetStats() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService[ "GetStats" ] as Estadisticas;
		}
	}

}