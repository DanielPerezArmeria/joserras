using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.exception;

namespace TorneosWeb.service.decorators
{
	public class CacheWrapperLigaReader : ILigaReader
	{
		private ICacheService cacheService;
		private ILigaReader wrapped;
		private ILogger<CacheWrapperLigaReader> log;

		public CacheWrapperLigaReader(ILigaReader reader, ICacheService cache, ILogger<CacheWrapperLigaReader> logger)
		{
			cacheService = cache;
			wrapped = reader;
			log = logger;
		}

		public Liga FindLigaByNombre(string nombre)
		{
			if(!cacheService.ContainsKey("Liga:" + nombre ) )
			{
				try
				{
					cacheService.Add( "Liga:" + nombre, wrapped.FindLigaByNombre( nombre ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<Liga>( "Liga:" + nombre );
		}

		public List<Liga> GetAllLigas()
		{
			if(!cacheService.ContainsKey( "GetAllLigas" ) )
			{
				try
				{
					cacheService.Add( "GetAllLigas", wrapped.GetAllLigas() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<List<Liga>>( "GetAllLigas" );
		}

		public Liga GetCurrentLiga()
		{
			if( !cacheService.ContainsKey( "GetCurrentLiga" ) )
			{
				try
				{
					cacheService.Add( "GetCurrentLiga", wrapped.GetCurrentLiga() );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<Liga>( "GetCurrentLiga" );
		}

		public List<Standing> GetStandings(Liga liga)
		{
			string key = "Standings:" + liga.Nombre;
			if(!cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetStandings( liga ) );
			}

			return cacheService.Get<List<Standing>>( key );
		}

		public List<Standing> GetStandings(Liga liga, Torneo torneo)
		{
			string key = "Standings:" + liga.Nombre + ":" + torneo.Id;
			if(!cacheService.ContainsKey( key ) )
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
				try
				{
					cacheService.Add( key, wrapped.GetLigaByTorneoId( torneoId ) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<Liga>( key );
		}

	}

}