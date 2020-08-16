using Microsoft.Extensions.Logging;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.exception;

namespace TorneosWeb.service.decorators
{
	public class TransactionWrapperStatsService : IStatsService
	{
		private IStatsService wrapped;
		private ICacheService cacheService;
		private ILogger<TransactionWrapperStatsService> log;

		public TransactionWrapperStatsService(IStatsService statsService, ICacheService cacheService, ILogger<TransactionWrapperStatsService> logger)
		{
			wrapped = statsService;
			this.cacheService = cacheService;
			this.log = logger;
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

		public Estadisticas GetStats(DateTime start, DateTime end)
		{
			string key = "GetStats:" + start.ToShortDateString() + ":" + end.ToShortDateString();
			if( !cacheService.Contains( key ) )
			{
				try
				{
					cacheService.Add( key, wrapped.GetStats(start, end) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<Estadisticas>( key );
		}

		public Estadisticas GetStats(Liga liga)
		{
			string key = "GetStats:" + liga.Nombre;
			if( !cacheService.Contains( key ) )
			{
				try
				{
					cacheService.Add( key, wrapped.GetStats(liga) );
				}
				catch( Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<Estadisticas>( key );
		}

	}

}