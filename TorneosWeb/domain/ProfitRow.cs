using System.Collections.Generic;

namespace TorneosWeb.domain
{
	public class ProfitRow
	{
		public string Nombre { get; set; }
		public int ProfitTotal { get; set; }

		public IList<object> ToList()
		{
			return new List<object> { this.Nombre, this.ProfitTotal };
		}
	}

}