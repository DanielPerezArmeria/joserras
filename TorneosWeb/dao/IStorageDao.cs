using System;
using System.Collections.Generic;
using TorneosWeb.domain.azure;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.dao
{
	public interface IStorageDao
	{
		void SaveTorneoStandings<T>(string tableName, Guid torneoId, List<Standing> standings) where T : AbstractPuntosAzureEntity;
	}

}