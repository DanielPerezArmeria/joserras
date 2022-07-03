using System;
using TorneosWeb.domain.models;

namespace TorneosWeb.util.disqualifiers
{
	public interface IDisqualifier
	{
		void Disqualify(DateTime lastDate, Estadisticas estadisticas);
	}

}