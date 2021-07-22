using Joserras.Commons.Domain;
using System.Collections.Generic;

namespace TorneosWeb.dao
{
	public interface IPrizeDao
	{
		IEnumerable<PrizeRange> GetPrizeRanges();
	}

}