using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class AsistenciaPointRule : PointRule
	{
		private int points;

		public AsistenciaPointRule(string Params)
		{
			points = int.Parse( Params );
		}

		public override int GetPuntos(Guid jugadorId, Liga liga, Resultados resultados)
		{
			return points;
		}

	}

}