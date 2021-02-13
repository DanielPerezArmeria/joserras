using System;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.tiebreakers
{
	public class KosDirectosComparer : AbstractStandingComparer
	{
		public override TiebreakerType Type => TiebreakerType.KOS_DIRECTOS;

		public override int Compare(Standing x, Standing y)
		{
			decimal kosXtoY = 0;
			try
			{
				kosXtoY = x.Liga.Estadisticas.Knockouts[ x.Jugador ][ y.Jugador ].EliminacionesNumber;
			}
			catch( Exception ) { }

			decimal kosYtoX = 0;
			try
			{
				kosYtoX = y.Liga.Estadisticas.Knockouts[ y.Jugador ][ x.Jugador ].EliminacionesNumber;
			}
			catch( Exception ) { }

			if( kosXtoY > kosYtoX )
			{
				return -1;
			}
			else if( kosXtoY == kosYtoX )
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

	}

}