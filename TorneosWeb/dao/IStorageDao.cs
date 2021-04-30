using System;
using System.Collections.Generic;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.dao
{
	public interface IStorageDao
	{
		void Init();

		void SaveTorneoStandings(string tableName, Guid torneoId, List<Standing> standings);
	}

}