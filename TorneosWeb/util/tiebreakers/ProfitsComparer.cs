using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.tiebreakers
{
	public class ProfitsComparer : AbstractStandingComparer
	{
		public override TiebreakerType Type => TiebreakerType.PROFIT;

		public override int Compare(Standing x, Standing y)
		{
			if( x.ProfitNumber > y.ProfitNumber )
			{
				return -1;
			}
			else if( x.ProfitNumber == y.ProfitNumber )
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