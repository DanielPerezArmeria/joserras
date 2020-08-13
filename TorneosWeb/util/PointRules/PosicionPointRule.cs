using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class PosicionPointRule : PointRule
	{
		private Dictionary<int, int> puntos = new Dictionary<int, int>();

		public PosicionPointRule(string Params)
		{
			string[] positionPairs = Params.Split( "|" );
			foreach(string pair in positionPairs )
			{
				string[] valores = pair.Split( "-" );
				puntos.Add( int.Parse( valores[ 0 ] ), int.Parse( valores[ 1 ] ) );
			}
		}

		public override int GetPuntos(Guid jugadorId, Liga liga, Resultados resultados)
		{
			Posicion posicion = resultados.Posiciones.Where( p => p.JugadorId == jugadorId ).First();
			if( puntos.ContainsKey( posicion.Lugar ) )
			{
				return puntos[ posicion.Lugar ];
			}

			return 0;
		}

	}

}