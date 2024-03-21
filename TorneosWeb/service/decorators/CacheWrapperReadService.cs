using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.decorators
{
	public class CacheWrapperReadService : IReadService
	{
		private IReadService wrapped;
		private ICacheService cacheService;
		private ILogger<CacheWrapperReadService> log;

		public CacheWrapperReadService(IReadService readService, ICacheService cacheService, ILogger<CacheWrapperReadService> logger)
		{
			wrapped = readService;
			this.cacheService = cacheService;
			log = logger;

			GetAllTorneos();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			string key = nameof( GetAllKnockouts );
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllKnockouts() );
			}

			return cacheService.Get<SortedList<string, Dictionary<string, Knockouts>>>( key );
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(DateTime start, DateTime end)
		{
			string key = "GetAllKnockouts:" + start.ToShortDateString() + ":" + end.ToShortDateString();
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllKnockouts( start, end ) );
			}
			return cacheService.Get<SortedList<string, Dictionary<string, Knockouts>>>( key );
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(Liga liga)
		{
			string key = string.Format( "{0}:{1}", nameof( GetAllKnockouts ), liga.Nombre );
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllKnockouts( liga ) );
			}
			return cacheService.Get<SortedList<string, Dictionary<string, Knockouts>>>( key );
		}

		public List<Torneo> GetAllTorneos()
		{
			string key = nameof( GetAllTorneos );

			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllTorneos() );
			}

			return cacheService.Get<List<Torneo>>( key );
		}

		public Resultados FindResultadosTorneo(Guid id)
		{
			string key = string.Format( "{0}:{1}", nameof( FindResultadosTorneo ), id.ToString() );

			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.FindResultadosTorneo( id ) );
			}

			return cacheService.Get<Resultados>( key );
		}

		public DetalleJugador FindDetalleJugador(Guid id)
		{
			string key = string.Format( "{0}:{1}", nameof( FindDetalleJugador ), id.ToString() );
			if ( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.FindDetalleJugador( id ) );
			}

			return cacheService.Get<DetalleJugador>( key );
		}

		public DetalleJugador FindDetalleJugador(string nombre)
		{
			throw new NotImplementedException();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockoutsByTournamentId(Guid torneoId)
		{
			string key = string.Format( "{0}:{1}", nameof( GetKnockoutsByTournamentId ), torneoId.ToString() );
			if ( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetKnockoutsByTournamentId( torneoId ) );
			}

			return cacheService.Get<SortedList<string, Dictionary<string, Knockouts>>>( key );
		}

		public List<Knockouts> GetKnockoutsByPlayer(Guid playerId)
		{
			string key = string.Format( "{0}:{1}", nameof( GetKnockoutsByPlayer ), playerId.ToString() );
			if ( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetKnockoutsByPlayer( playerId ) );
			}

			return cacheService.Get<List<Knockouts>>( key );
		}

		public List<DetalleJugador> GetAllDetalleJugador()
		{
			string key = nameof( GetAllDetalleJugador );
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllDetalleJugador() );
			}

			return cacheService.Get<List<DetalleJugador>>( key );
		}

		public List<DetalleJugador> GetAllDetalleJugador(DateTime start, DateTime end)
		{
			string key = "GetAllDetalleJugador:" + start.ToShortDateString() + ":" + end.ToShortDateString();
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllDetalleJugador( start, end ) );
			}

			return cacheService.Get<List<DetalleJugador>>( key );
		}

		public List<DetalleJugador> GetAllDetalleJugador(Liga liga)
		{
			string key = "GetAllDetalleJugador:" + liga.Nombre;
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllDetalleJugador( liga ) );
			}

			return cacheService.Get<List<DetalleJugador>>( key );
		}

		public Torneo FindTorneoByFecha(DateTime fecha)
		{
			string key = string.Format( "{0}:{1}", nameof( FindTorneoByFecha ), fecha.ToShortDateString() );

			if (!cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.FindTorneoByFecha( fecha ) );
			}

			return cacheService.Get<Torneo>( key );
		}

		public List<Knockouts> GetTournamentKOList(Guid torneoId)
		{
			string key = string.Format( "{0}:{1}", nameof( GetTournamentKOList ), torneoId.ToString() );
			if(!cacheService.ContainsKey( key ))
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetTournamentKOList( torneoId ) );
			}

			return cacheService.Get<List<Knockouts>>( key );
		}

	}

}