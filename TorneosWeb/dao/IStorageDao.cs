using System;
using System.Collections.Generic;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.dao
{
	public interface IStorageDao
	{
		void SaveTorneoStandings(Guid id, List<Standing> standings);

		void SaveLigaStandings(Guid id, List<Standing> standings);
	}

}