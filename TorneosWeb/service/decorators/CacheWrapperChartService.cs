using Microsoft.Extensions.Logging;
using System;
using TorneosWeb.domain.charts;
using TorneosWeb.exception;

namespace TorneosWeb.service.decorators
{
	public class CacheWrapperChartService : IChartService
	{
		private ICacheService cacheService;
		private IChartService wrapped;
		private ILogger<CacheWrapperChartService> log;

		public CacheWrapperChartService(ICacheService cache, IChartService wrapped, ILogger<CacheWrapperChartService> logger)
		{
			cacheService = cache;
			this.wrapped = wrapped;
			log = logger;
		}

		public ProfitChartItem GetProfitHistoryByPlayerId(Guid playerId)
		{
			string key = nameof( GetProfitHistoryByPlayerId ) + ":" + playerId;
			if(!cacheService.Contains( key ) )
			{
				try
				{
					cacheService.Add( key, wrapped.GetProfitHistoryByPlayerId( playerId ) );
				}
				catch(Exception e )
				{
					log.LogWarning( e, e.Message );
					throw new JoserrasException( e );
				}
			}

			return cacheService.Get<ProfitChartItem>( key );
		}

	}

}