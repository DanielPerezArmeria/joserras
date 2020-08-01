using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class KosPointRule : PointRule
	{
		private int puntos;

		public KosPointRule(params string[] Params)
		{
			puntos = int.Parse( Params[ 1 ] );
		}

		public override int GetPuntos(Guid jugadorId, Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
		{
			ResultadosDTO res = resultados.Where( p => p.JugadorId == jugadorId ).First();
			return puntos * res.Kos;
		}

	}

}