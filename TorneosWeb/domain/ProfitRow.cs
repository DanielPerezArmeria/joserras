using System;
using System.Collections.Generic;

namespace TorneosWeb.domain
{
	public class ProfitRow
	{
		public ProfitRow()
		{
			Profits = new List<KeyValuePair<Guid, decimal>>();
		}

		public string Nombre { get; set; }
		public List<KeyValuePair<Guid, decimal>> Profits { get; set; }
	}

}