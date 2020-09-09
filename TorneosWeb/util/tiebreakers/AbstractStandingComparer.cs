using Humanizer;
using System.Collections.Generic;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.tiebreakers
{
	public abstract class AbstractStandingComparer : Comparer<Standing>
	{
		public abstract TiebreakerType Type { get; }

		public virtual string Description
		{
			get
			{
				return Type.Humanize();
			}
		}
	}

}