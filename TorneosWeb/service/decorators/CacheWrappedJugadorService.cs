using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using Microsoft.Extensions.Logging;

namespace TorneosWeb.service.decorators
{
	public class CacheWrappedJugadorService : IJugadorService
	{
		private ICacheService cacheService;
		private IJugadorService wrapped;
		private ILogger<CacheWrappedJugadorService> log;

		public CacheWrappedJugadorService(ICacheService cacheService, IJugadorService wrapped, ILogger<CacheWrappedJugadorService> log)
		{
			this.cacheService = cacheService;
			this.wrapped = wrapped;
			this.log = log;
		}

		public List<Jugador> GetAllJugadores()
		{
			string key = nameof( GetAllJugadores );
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllJugadores() );
			}

			return cacheService.Get<List<Jugador>>( key );
		}

		public List<Posicion> GetAllPosicionesByJugador(Guid jugadorId)
		{
			string key = string.Format( "{0}:{1}", nameof( GetAllPosicionesByJugador ), jugadorId.ToString() );
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetAllPosicionesByJugador( jugadorId ) );
			}
			return cacheService.Get<List<Posicion>>( key );
		}

	}

}