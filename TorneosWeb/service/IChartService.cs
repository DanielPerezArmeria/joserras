using System;
using System.Collections.Generic;
using TorneosWeb.domain;

namespace TorneosWeb.service
{
	public interface IChartService
	{
		List<ChartDataPoint> GetPlayerProfitChartData(Guid playerId);
	}

}