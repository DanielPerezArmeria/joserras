using System;
using TorneosWeb.domain.charts;

namespace TorneosWeb.service
{
	public interface IChartService
	{
		ProfitHistory GetProfitHistoryByPlayerId(Guid playerId);
	}

}