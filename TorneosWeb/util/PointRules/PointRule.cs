using System;
using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public abstract class PointRule
	{
		public PointRuleType Type { get; set; }

		public abstract int GetPuntos(Guid jugadorId, Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos);

		public static PointRule Build(string rule)
		{
			return null;
		}

	}

}