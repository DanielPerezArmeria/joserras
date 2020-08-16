using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service
{
	public interface IStatsService
	{
		Estadisticas GetStats();
		Estadisticas GetStats(DateTime start, DateTime end);
		Estadisticas GetStats(Liga liga);
	}

}