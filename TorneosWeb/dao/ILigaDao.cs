using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.dao
{
	public interface ILigaDao
	{
		Liga GetLigaByTorneoId(Guid torneoId);

		LigaProfitsObject GetTotalLigaProfitsByPlayerId(Guid playerId);

		IEnumerable<LigaProfitsObject> GetTotalLigaProfits();

		IEnumerable<LigaProfitsObject> GetLigaProfitsByLiga(Liga liga);
	}

}