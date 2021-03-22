using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.domain.models.ligas
{
	public class Standing
	{
		public Standing()
		{
			Puntos = new SortedDictionary<PointRuleType, FDecimal>();
		}

		public Liga Liga { get; set; }

		public string Jugador { get; set; }
		public Guid JugadorId { get; set; }
		public SortedDictionary<PointRuleType, FDecimal> Puntos { get; set; }
		public decimal Total
		{
			get
			{
				return Puntos.Sum( p => p.Value );
			}
		}

		public string TotalString { get { return Total.ToString( Constants.POINTS_FORMAT ); } }

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

		[Display( Name = "Premio Liga" )]
		public string PremioLiga
		{
			get
			{
				if(PremioLigaNumber > 0 )
				{
					return PremioLigaNumber.ToString( Constants.CURRENCY_FORMAT );
				}
				else
				{
					return "-";
				}
			}
		}

		public decimal PremioLigaNumber { get; set; }

	}

}