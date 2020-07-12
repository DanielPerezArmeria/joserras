using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class DetalleJugador
	{
		public Guid Id { get; set; }
		public string Nombre { get; set; }
		public int ProfitNumber { get; set; }
		[NoMap]
		public string Profit
		{
			get { return ProfitNumber.ToString( Constants.CURRENCY_FORMAT ); }
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
			get { return PremiosNumber.ToString( Constants.CURRENCY_FORMAT ); }
		}
		public int PremiosNumber { get; set; }
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
				return (ProfitNumber / (float)CostosNumber).ToString( "%###,###.##" );
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
	}

}