﻿using System;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class Posicion : IComparable<Posicion>
	{
		public Guid TorneoId { get; set; }
		public Guid JugadorId { get; set; }
		public string Nombre { get; set; }
		public int Lugar { get; set; }

		[NoMap]
		public Torneo Torneo { get; set; }

		public decimal PremioNumber { get; set; }

		[NoMap]
		[Display(Name = "$ Premio")]
		public string Premio { get { return PremioNumber > 0 ? PremioNumber.ToString( Constants.CURRENCY_FORMAT ) : "-"; } }

		public bool Podio { get; set; }
		public int Rebuys { get; set; }
		public bool Burbuja { get; set; }

		public decimal KnockoutsNumber { get; set; }

		[NoMap]
		public string Knockouts
		{
			get { return KnockoutsNumber.ToString( Constants.POINTS_FORMAT ); }
		}

		[NoMap]
		[Display( Name = "Profit Torneo" )]
		public string ProfitTorneo { get { return ProfitTorneoNumber != 0 ? ProfitTorneoNumber.ToString( Constants.CURRENCY_FORMAT ) : "$0"; } }

		[NoMap]
		public decimal ProfitTorneoNumber
		{
			get { return PremioNumber - TorneoCostos; }
		}

		[NoMap]
		[Display( Name = "Profit Total" )]
		public string ProfitTotal { get { return ProfitTotalNumber != 0 ? ProfitTotalNumber.ToString( Constants.CURRENCY_FORMAT ) : "$0"; } }

		[NoMap]
		public decimal ProfitTotalNumber
		{
			get
			{
				return ProfitTorneoNumber + PremioBountiesNumber - LigaCostos;
			}
		}

		[NoMap]
		public decimal LigaCostos
		{
			get
			{
				if(Torneo.Liga != null )
				{
					return (Rebuys + 1) * Torneo.Liga.Fee;
				}

				return 0;
			}
		}

		[NoMap]
		public decimal TorneoCostos
		{
			get { return Torneo.PrecioBuyinNumber + (Rebuys * Torneo.PrecioRebuyNumber); }
		}

		[NoMap]
		public Resultados Resultados { get; set; }

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