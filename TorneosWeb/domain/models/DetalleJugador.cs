using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class DetalleJugador : IComparable<DetalleJugador>
	{
		public Guid Id { get; set; }
		public string Nombre { get; set; }

		[NoMap]
		public decimal ProfitNumber
		{
			get
			{
				return PremiosNumber + PremiosLigaNumber - CostosNumber;
			}
		}

		[NoMap]
		public string Profit
		{
			get { return ProfitNumber != 0 ? ProfitNumber.ToString( Constants.CURRENCY_FORMAT ) : "$0"; }
		}

		[NoMap]
		public List<Knockouts> Knockouts { get; set; }
		public int Torneos { get; set; }
		public int Rebuys { get; set; }
		public int Podios { get; set; }
		public int Burbujas { get; set; }
		public int Victorias { get; set; }
		[NoMap]
		public string Premios
		{
			get { return PremiosNumber > 0 ? PremiosNumber.ToString( Constants.CURRENCY_FORMAT ) : "$0"; }
		}
		public decimal PremiosNumber { get; set; }
		[NoMap]
		public string Costos
		{
			get { return CostosNumber.ToString( Constants.CURRENCY_FORMAT ); }
		}
		public int CostosNumber { get; set; }
		[Display(Name = "KO's")]
		public int Kos { get; set; }

		[NoMap]
		public string ROI
		{
			get
			{
				return ROINumber != 0 ? ((float)ProfitNumber / CostosNumber).ToString( "%###,###.#" ) : "%0.0";
			}
		}

		[NoMap]
		public float ROINumber
		{
			get
			{
				return (float)ProfitNumber / CostosNumber;
			}
		}

		[NoMap]
		public decimal PremiosLigaNumber { get; set; }

		[NoMap]
		[Display( Name = "Premios Liga" )]
		public string PremiosLiga
		{
			get
			{
				if(PremiosLigaNumber > 0 )
				{
					return PremiosLigaNumber.ToString( Constants.CURRENCY_FORMAT );
				}
				return "-";
			}
		}

		public int CompareTo(DetalleJugador other)
		{
			if( this.ProfitNumber > other.ProfitNumber )
			{
				return -1;
			}
			else if( this.ProfitNumber == other.ProfitNumber )
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

	}

}