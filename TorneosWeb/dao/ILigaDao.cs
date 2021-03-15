using System;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.dao
{
	public interface ILigaDao
	{
		Liga GetLigaByTorneoId(Guid torneoId);
	}

}