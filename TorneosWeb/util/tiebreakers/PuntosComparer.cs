using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.tiebreakers
{
	public class PuntosComparer : AbstractStandingComparer
	{
		public override TiebreakerType Type => TiebreakerType.PUNTOS;

		public override int Compare(Standing x, Standing y)
		{
			if( x.Total > y.Total )
			{
				return -1;
			}
			else if( x.Total == y.Total )
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