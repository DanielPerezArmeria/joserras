using System;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class KosPointRule : PointRule
	{
		public KosPointRule(string Params)
		{
			points = decimal.Parse( Params );
		}

		public override PointRuleType Type => PointRuleType.KO;

		public override decimal GetPuntaje(Guid jugadorId, Liga liga, Resultados resultados)
		{
			Posicion posicion = resultados.Posiciones.Where( p => p.JugadorId == jugadorId ).First();
			return points * posicion.KnockoutsNumber;
		}

	}

}