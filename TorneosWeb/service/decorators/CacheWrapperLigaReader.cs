using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.decorators
{
	public class CacheWrapperLigaReader : ILigaReader
	{
		private ICacheService cacheService;
		private ILigaReader wrapped;

		public CacheWrapperLigaReader(ILigaReader reader, ICacheService cache)
		{
			cacheService = cache;
			wrapped = reader;
		}

		public Liga FindLigaByNombre(string nombre)
		{
			string key = string.Format( "{0}:{1}", nameof( FindLigaByNombre ), nombre );
			if(!cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.FindLigaByNombre( nombre ) );
			}

			return cacheService.Get<Liga>( key );
		}

		public List<Liga> GetAllLigas()
		{
			string key = nameof( GetAllLigas );
			if (!cacheService.ContainsKey( "GetAllLigas" ) )
			{
				cacheService.Add( key, wrapped.GetAllLigas() );
			}

			return cacheService.Get<List<Liga>>( key );
		}

		public Liga GetCurrentLiga()
		{
			string key = nameof( GetCurrentLiga );
			if ( !cacheService.ContainsKey( "GetCurrentLiga" ) )
			{
				cacheService.Add( key, wrapped.GetCurrentLiga() );
			}

			return cacheService.Get<Liga>( key );
		}

		public List<Standing> GetStandings(Liga liga)
		{
			string key = string.Format( "{0}:{1}", nameof( GetStandings ), liga.Id );
			if (!cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetStandings( liga ) );
			}

			return cacheService.Get<List<Standing>>( key );
		}

		public List<Standing> GetStandings(Liga liga, Torneo torneo)
		{
			string key = string.Format( "{0}:{1}:{2}", nameof( GetStandings ), liga.Id, torneo.Id );
			if (!cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetStandings( liga, torneo ) );
			}

			return cacheService.Get<List<Standing>>( key );
		}

		public Liga GetLigaByTorneoId(Guid torneoId)
		{
			string key = nameof( GetLigaByTorneoId ) + ":" + torneoId;
			if( !cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetLigaByTorneoId( torneoId ) );
			}

			return cacheService.Get<Liga>( key );
		}

	}

}