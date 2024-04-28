using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class DetalleJugador : IComparable<DetalleJugador>, IEquatable<DetalleJugador>
	{
		public Guid Id { get; set; }
		public string Nombre { get; set; }

		[NoMap]
		public decimal ProfitNumber
		{
			get
			{
				return ProfitTorneosNumber + ProfitLigasNumber;
			}
		}

		[NoMap]
		public string Profit
		{
			get { return ProfitNumber != 0 ? ProfitNumber.ToString( Constants.CURRENCY_FORMAT ) : "$0"; }
		}

		[NoMap]
		public decimal ProfitTorneosNumber
		{
			get { return PremiosNumber - CostosNumber; }
		}

		[NoMap]
		[Display( Name = "Profit Torneos" )]
		public string ProfitTorneos
		{
			get { return ProfitTorneosNumber != 0 ? ProfitTorneosNumber.ToString( Constants.CURRENCY_FORMAT ) : "$0"; }
		}

		[NoMap]
		public List<Knockouts> Knockouts { get; set; }
		public int Torneos { get; set; }
		public int Rebuys { get; set; }
		public int Podios { get; set; }
		public int Burbujas { get; set; }
		public int Victorias { get; set; }

		[NoMap]
		[Display( Name = "Último Lugar" )]
		public int UltimoLugar { get; set; }

		[NoMap]
		[Display( Name = "Podios Negativos" )]
		public int PodiosNegativos { get; set; }

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

		[NoMap]
		public decimal CostosTotalesNumber { get { return CostosNumber + CostosLigaNumber; } }
		[NoMap]
		public string CostosTotales { get { return CostosTotalesNumber.ToString( Constants.CURRENCY_FORMAT ); } }

		public decimal KosNumber { get; set; }

		[Display( Name = "KO's" )]
		[NoMap]
		public string Kos
		{
			get { return  KosNumber.ToString( Constants.POINTS_FORMAT ); }
		}

		[Display( Name = "ROI" )]
		[NoMap]
		public string ROI
		{
			get
			{
				return ROINumber != 0 ? ROINumber.ToString( Constants.ROI_FORMAT ) : "%0.0";
			}
		}

		[NoMap]
		public float ROINumber
		{
			get
			{
				return (float)ProfitNumber / (float)CostosTotalesNumber;
			}
		}

		[NoMap]
		public decimal PremiosLigaNumber { get; set; }
		[NoMap]
		public string PremiosLiga
		{
			get { return PremiosLigaNumber.ToString( Constants.CURRENCY_FORMAT ); }
		}

		[NoMap]
		public decimal CostosLigaNumber { get; set; }

		[NoMap]
		public decimal ProfitLigasNumber { get { return PremiosLigaNumber - CostosLigaNumber; } }

		[NoMap]
		[Display( Name = "Profits Liga" )]
		public string ProfitLigas
		{
			get
			{
				if( ProfitLigasNumber != 0 )
				{
					return ProfitLigasNumber.ToString( Constants.CURRENCY_FORMAT );
				}
				return "$0";
			}
		}

		[NoMap]
		[Display( Name = "ITM %" )]
		public string Itm { get { return ( (float)Podios / (float)Torneos ).ToString( Constants.ROI_FORMAT ); } }

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

		public bool Equals(DetalleJugador other)
		{
			return Id.Equals( other.Id );
		}

	}

}