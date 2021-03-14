using System;
using TorneosWeb.domain.charts;

namespace TorneosWeb.service
{
	public interface IChartService
	{
		ProfitChartItem GetProfitHistoryByPlayerId(Guid playerId);
	}

}