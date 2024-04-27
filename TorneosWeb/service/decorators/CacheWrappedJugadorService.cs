using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service.decorators
{
	public class CacheWrappedJugadorService : IJugadorService
	{
		private ICacheService cacheService;
		private IJugadorService wrapped;

		public CacheWrappedJugadorService(ICacheService cacheService, IJugadorService wrapped)
		{
			this.cacheService = cacheService;
			this.wrapped = wrapped;
		}

		public List<Jugador> GetAllJugadores()
		{
			string key = nameof( GetAllJugadores );
			if( !cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetAllJugadores() );
			}

			return cacheService.Get<List<Jugador>>( key );
		}

		public List<Posicion> GetAllPosicionesByJugador(Guid jugadorId)
		{
			string key = string.Format( "{0}:{1}", nameof( GetAllPosicionesByJugador ), jugadorId.ToString() );
			if( !cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetAllPosicionesByJugador( jugadorId ) );
			}
			return cacheService.Get<List<Posicion>>( key );
		}

	}

}