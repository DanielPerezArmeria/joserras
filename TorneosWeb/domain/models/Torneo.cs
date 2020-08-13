using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class Torneo
	{
		private IList<Posicion> posiciones;

		public Guid Id { get; set; }

		[NoMap]
		public string Fecha { get { return FechaDate.ToShortDateString(); } }
		public DateTime FechaDate { get; set; }

		[Display( Name = "$ Buyin" )]
		[NoMap]
		public string Precio_Buyin
		{
			get
			{
				int total = PrecioBuyinNumber - PremioBountyNumber - (Liga == null ? 0 : Liga.Fee);
				string buyin = total.ToString( Constants.CURRENCY_FORMAT );
				if( Tipo == TournamentType.BOUNTY )
				{
					buyin = buyin + " + " + PremioBountyNumber.ToString( Constants.CURRENCY_FORMAT ) + "(B)";
				}
				if( Liga != null )
				{
					buyin = buyin + " + " + Liga.Fee.ToString( Constants.CURRENCY_FORMAT ) + "(L)";
				}
				return buyin;
			}
		}

		public int PrecioBuyinNumber { get; set; }

		[Display( Name = "$ Re-buy" )]
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
		[Display( Name = "Bounty" )]
		public string PremioBounty { get { return PremioBountyNumber > 0 ? PremioBountyNumber.ToString( "$###,###" ) : "-"; } }

		[NoMap]
		public Liga Liga { get; set; }

		[NoMap]
		IList<Posicion> Posiciones { get { return posiciones; } set { posiciones = value; } }

	}

}