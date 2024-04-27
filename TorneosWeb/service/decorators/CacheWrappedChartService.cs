using System;

namespace TorneosWeb.service.decorators
{
	public class CacheWrappedChartService : IChartService
	{
		private IChartService wrapped;
		private ICacheService cacheService;

		public CacheWrappedChartService(IChartService wrapped, ICacheService cacheService)
		{
			this.wrapped = wrapped;
			this.cacheService = cacheService;
		}

		public string GetPlayerProfitChartData(Guid playerId)
		{
			string key = string.Format( "{0}:{1}", nameof( GetPlayerProfitChartData ), playerId.ToString() );
			if( !cacheService.ContainsKey( key ) )
			{
				cacheService.Add( key, wrapped.GetPlayerProfitChartData( playerId ) );
			}
			return cacheService.Get<string>( key );
		}

	}

}