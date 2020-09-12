using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.domain.models.ligas
{
	public class Standing
	{
		public Standing()
		{
			Puntos = new SortedDictionary<PointRuleType, int>();
		}

		public Liga Liga { get; set; }

		public string Jugador { get; set; }
		public Guid JugadorId { get; set; }
		public SortedDictionary<PointRuleType,int> Puntos { get; set; }
		public int Total
		{
			get
			{
				return Puntos.Sum( p => p.Value );
			}
		}

		public string Profit
		{
			get
			{
				if( ProfitNumber != 0 )
				{
					return ProfitNumber.ToString( Constants.CURRENCY_FORMAT );
				}
				else
				{
					return "$0";
				}
			}
		}

		public decimal ProfitNumber { get; set; }

		public string Premio
		{
			get
			{
				if(PremioNumber > 0 )
				{
					return PremioNumber.ToString( Constants.CURRENCY_FORMAT );
				}
				else
				{
					return "-";
				}
			}
		}

		public decimal PremioNumber { get; set; }

	}

}