using System.Collections.Generic;
using System.Linq;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.domain.models.ligas
{
	public class Standing
	{
		public Standing()
		{
			Puntos = new SortedDictionary<PointRuleType, int>();
		}

		public string Jugador { get; set; }
		public SortedDictionary<PointRuleType,int> Puntos { get; set; }
		public int Total
		{
			get
			{
				return Puntos.Sum( p => p.Value );
			}
		}

		public string Profit { get; set; }
		public int ProfitNumber { get; set; }

	}

}