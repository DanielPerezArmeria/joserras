using Microsoft.Extensions.Logging;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.decorators
{
	public class CacheWrapperStatsService : IStatsService
	{
		private IStatsService wrapped;
		private ICacheService cacheService;
		private ILogger<CacheWrapperStatsService> log;

		public CacheWrapperStatsService(IStatsService statsService, ICacheService cacheService, ILogger<CacheWrapperStatsService> logger)
		{
			wrapped = statsService;
			this.cacheService = cacheService;
			this.log = logger;
		}

		public Estadisticas GetStats()
		{
			string key = nameof( GetStats );
			if( !cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetStats() );
			}

			return cacheService.Get<Estadisticas>( key );
		}

		public Estadisticas GetStats(DateTime start, DateTime end)
		{
			string key = "GetStats:" + start.ToShortDateString() + ":" + end.ToShortDateString();
			if( !cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetStats(start, end) );
			}

			return cacheService.Get<Estadisticas>( key );
		}

		public Estadisticas GetStats(Liga liga)
		{
			string key = "GetStats:" + liga.Nombre;
			if( !cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetStats(liga) );
			}

			return cacheService.Get<Estadisticas>( key );
		}

	}

}