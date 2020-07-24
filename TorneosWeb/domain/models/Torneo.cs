using System;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class Torneo
	{
		public Guid Id { get; set; }
		public string Fecha { get; set; }

		[Display( Name = "$ Buyin" )]
		[NoMap]
		public string Precio_Buyin
		{
			get
			{
				string buyin = PrecioBuyinNumber.ToString( Constants.CURRENCY_FORMAT );
				if(Tipo == TournamentType.BOUNTY )
				{
					buyin = (PrecioBuyinNumber - PremioBountyNumber).ToString( Constants.CURRENCY_FORMAT );
					buyin = buyin + " + " + PremioBountyNumber.ToString( Constants.CURRENCY_FORMAT );
				}
				return buyin;
			}
		}

		public int PrecioBuyinNumber { get; set; }

		[Display( Name = "$ Re-buy")]
		[NoMap]
		public string Precio_Rebuy { get { return PrecioRebuyNumber.ToString( Constants.CURRENCY_FORMAT ); } }

		public int PrecioRebuyNumber { get; set; }

		public int Entradas { get; set; }
		public int Rebuys { get; set; }
		public string Bolsa { get; set; }
		public string Ganador { get; set; }
		public Guid GanadorId { get; set; }
		public TournamentType Tipo { get; set; }
		public int PremioBountyNumber { get; set; }

		[NoMap]
		[Display(Name = "Bounty")]
		public string PremioBounty { get { return PremioBountyNumber > 0 ? PremioBountyNumber.ToString( "$###,###" ) : "-"; } }
	}

}