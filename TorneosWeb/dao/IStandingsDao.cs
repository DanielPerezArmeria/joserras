using System;
using System.Collections.Generic;
using TorneosWeb.domain.azure;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.dao
{
	public interface IStandingsDao<T> where T : AbstractPuntosAzureEntity
	{
		void Save(Guid id, List<Standing> standings);
	}

}