using System.Collections.Generic;
using TorneosWeb.domain.dto;

namespace TorneosWeb.dao
{
	public interface IPrizeDao
	{
		IEnumerable<PrizeRange> GetPrizeRanges();
	}

}