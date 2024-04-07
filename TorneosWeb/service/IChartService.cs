using System;

namespace TorneosWeb.service
{
	public interface IChartService
	{
		string GetPlayerProfitChartData(Guid playerId);
	}

}