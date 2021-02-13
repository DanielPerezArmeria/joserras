using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class AsistenciaPointRule : PointRule
	{
		public AsistenciaPointRule(string Params)
		{
			points = decimal.Parse( Params );
		}

		public override PointRuleType Type => PointRuleType.ASISTENCIA;

		public override decimal GetPuntaje(Guid jugadorId, Liga liga, Resultados resultados)
		{
			return points;
		}

	}

}