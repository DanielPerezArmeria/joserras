using Microsoft.Extensions.Logging;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.exception;

namespace TorneosWeb.service.impl
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

	}

}