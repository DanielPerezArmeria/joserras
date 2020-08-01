using System;
using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class AsistenciaPointRule : PointRule
	{
		private int points;

		public AsistenciaPointRule(params string[] Params)
		{
			points = int.Parse( Params[1] );
		}

		public override int GetPuntos(Guid jugadorId, Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
		{
			return points;
		}

	}

}