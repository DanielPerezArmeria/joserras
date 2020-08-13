using System;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class KosPointRule : PointRule
	{
		private int puntos;

		public KosPointRule(string Params)
		{
			puntos = int.Parse( Params );
		}

		public override int GetPuntos(Guid jugadorId, Liga liga, Resultados resultados)
		{
			Posicion posicion = resultados.Posiciones.Where( p => p.JugadorId == jugadorId ).First();
			return puntos * posicion.Knockouts;
		}

	}

}