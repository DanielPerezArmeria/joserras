using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service
{
	public interface IProfitsExporter
	{
		void ExportProfits(List<Torneo> torneos);
	}

}