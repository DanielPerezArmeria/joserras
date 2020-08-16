using System;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class PuntualidadPointRule : PointRule
	{
		public PuntualidadPointRule(string Params)
		{
			points = int.Parse( Params );
		}

		public override PointRuleType Type => PointRuleType.PUNTUALIDAD;

		public override int GetPuntaje(Guid jugadorId, Liga liga, Resultados resultados)
		{
			Posicion posicion = resultados.Posiciones.Where( p => p.JugadorId == jugadorId ).First();
			if( posicion.Puntualidad )
			{
				return points;
			}

			return 0;
		}

	}

}