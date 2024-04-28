using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.util.disqualifiers
{
	public interface IDisqualifier
	{
		IList<Guid> Disqualify(DateTime lastDate, Estadisticas estadisticas);
	}

}