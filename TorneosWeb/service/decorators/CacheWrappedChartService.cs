using Microsoft.Extensions.Logging;
using System;

namespace TorneosWeb.service.decorators
{
	public class CacheWrappedChartService : IChartService
	{
		private IChartService wrapped;
		private ICacheService cacheService;
		private ILogger<CacheWrappedChartService> log;

		public CacheWrappedChartService(IChartService wrapped, ICacheService cacheService, ILogger<CacheWrappedChartService> log)
		{
			this.wrapped = wrapped;
			this.cacheService = cacheService;
			this.log = log;
		}

		public string GetPlayerProfitChartData(Guid playerId)
		{
			string key = string.Format( "{0}:{1}", nameof( GetPlayerProfitChartData ), playerId.ToString() );
			if( !cacheService.ContainsKey( key ) )
			{
				log.LogDebug( "Key '{0}' was not found in cache. Calling service.", key );
				cacheService.Add( key, wrapped.GetPlayerProfitChartData( playerId ) );
			}
			return cacheService.Get<string>( key );
		}

	}

}