using System.Collections.Generic;

namespace TorneosWeb.domain
{
	public class ProfitRow
	{
		public ProfitRow()
		{
			Profits = new Dictionary<string, decimal>();
		}

		public string Nombre { get; set; }
		public IDictionary<string, decimal> Profits { get; set; }
	}

}