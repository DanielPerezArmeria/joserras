using System;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class Posicion : IComparable<Posicion>
	{
		public Guid JugadorId { get; set; }
		public string Nombre { get; set; }
		public int Lugar { get; set; }

		public decimal PremioNumber { get; set; }

		[NoMap]
		[Display(Name = "$ Premio")]
		public string Premio { get { return PremioNumber > 0 ? PremioNumber.ToString( Constants.CURRENCY_FORMAT ) : "-"; } }

		public string Podio { get; set; }
		public int Rebuys { get; set; }
		public string Burbuja { get; set; }

		public decimal KnockoutsNumber { get; set; }

		[NoMap]
		public string Knockouts
		{
			get { return decimal.Ceiling(KnockoutsNumber).ToString( Constants.KOS_FORMAT ); }
		}

		[NoMap]
		public string Profit { get { return ProfitNumber != 0 ? ProfitNumber.ToString( Constants.CURRENCY_FORMAT ) : "$0"; } }

		[NoMap]
		public decimal ProfitNumber { get; set; }

		public bool Puntualidad { get; set; }

		public int PremioBountiesNumber { get; set; }

		[NoMap]
		[Display(Name = "$ Bounties")]
		public string PremioBounties { get { return PremioBountiesNumber > 0 ? PremioBountiesNumber.ToString( Constants.CURRENCY_FORMAT ) : "-"; } }

		public int CompareTo(Posicion other)
		{
			if( Lugar > other.Lugar )
			{
				return 1;
			}
			else if( Lugar == other.Lugar )
			{
				return 0;
			}
			else
			{
				return -1;
			}
		}

	}

}