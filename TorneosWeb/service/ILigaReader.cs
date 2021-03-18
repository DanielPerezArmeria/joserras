using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service
{
	public interface ILigaReader
	{
		Liga FindLigaByNombre(string nombre);

		Liga GetCurrentLiga();

		List<Liga> GetAllLigas();

		List<Standing> GetStandings(Liga liga);

		List<Standing> GetStandings(Liga liga, Torneo torneo);

		Liga GetLigaByTorneoId(Guid torneoId);
	}

}