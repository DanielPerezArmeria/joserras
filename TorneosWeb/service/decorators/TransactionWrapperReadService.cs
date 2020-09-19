using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.exception;

namespace TorneosWeb.service.decorators
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

			GetAllTorneos();
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

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(DateTime start, DateTime end)
		{
			string key = "GetAllKnockouts:" + start.ToShortDateString() + ":" + end.ToShortDateString();
			if( !cacheService.Contains( key ) )
			{
				cacheService.Add( key, wrapped.GetAllKnockouts( start, end ) );
			}
			return cacheService.Get<SortedList<string, Dictionary<string, Knockouts>>>( key );
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(Liga liga)
		{
			string key = "GetAllKnockouts:" + liga.Nombre;
			if( !cacheService.Contains( key ) )
			{
				cacheService.Add( key, wrapped.GetAllKnockouts( liga ) );
			}
			return cacheService.Get<SortedList<string, Dictionary<string, Knockouts>>>( key );
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

		public Resultados FindResultadosTorneo(Guid id)
		{
			if( !cacheService.GetDetalleTorneo.ContainsKey( id ) )
			{
				try
				{
					cacheService.GetDetalleTorneo.Add( id, wrapped.FindResultadosTorneo( id ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetDetalleTorneo[ id ];
		}

		public DetalleJugador FindDetalleJugador(Guid id)
		{
			if( !cacheService.GetDetalleJugadorById.ContainsKey( id ) )
			{
				try
				{
					cacheService.GetDetalleJugadorById.Add( id, wrapped.FindDetalleJugador( id ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetDetalleJugadorById[ id ];
		}

		public DetalleJugador FindDetalleJugador(string nombre)
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

		public List<DetalleJugador> GetAllDetalleJugador()
		{
			if( cacheService.GetDetalleJugador.Count == 0 )
			{
				try
				{
					cacheService.GetDetalleJugador.AddRange( wrapped.GetAllDetalleJugador() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.GetDetalleJugador;
		}

		public List<DetalleJugador> GetAllDetalleJugador(DateTime start, DateTime end)
		{
			string key = "GetAllDetalleJugador:" + start.ToShortDateString() + ":" + end.ToShortDateString();
			if( !cacheService.Contains( key ) )
			{
				try
				{
					cacheService.Add( key, wrapped.GetAllDetalleJugador( start, end ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<List<DetalleJugador>>( key );
		}

		public List<DetalleJugador> GetAllDetalleJugador(Liga liga)
		{
			string key = "GetAllDetalleJugador:" + liga.Nombre;
			if( !cacheService.Contains( key ) )
			{
				try
				{
					cacheService.Add( key, wrapped.GetAllDetalleJugador( liga ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<List<DetalleJugador>>( key );
		}

		public Torneo FindTorneoByFecha(DateTime fecha)
		{
			Torneo torneo = cacheService.Get<Torneo>( "Torneo" + fecha.ToShortDateString() );

			if( torneo == null )
			{
				try
				{
					cacheService.Add( "Torneo" + fecha.ToShortDateString(), wrapped.FindTorneoByFecha( fecha ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<Torneo>( "Torneo" + fecha.ToShortDateString() );
		}

		public Torneo FindTorneoById(Guid id)
		{
			Torneo torneo = cacheService.Get<Torneo>( "Torneo" + id.ToString() );

			if( torneo == null )
			{
				try
				{
					cacheService.Add( "Torneo" + id.ToString(), wrapped.FindTorneoById( id ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<Torneo>( "Torneo" + id.ToString() );
		}

	}

}