using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class AsistenciaPointRule : PointRule
	{
		public AsistenciaPointRule(string Params)
		{
			points = int.Parse( Params );
		}

		public override PointRuleType Type => PointRuleType.ASISTENCIA;

		public override int GetPuntaje(Guid jugadorId, Liga liga, Resultados resultados)
		{
			return points;
		}

	}

}