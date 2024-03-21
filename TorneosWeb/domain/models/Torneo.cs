using Humanizer;
using Joserras.Commons.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models
{
	public class Torneo
	{
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
				int total = PrecioBuyinNumber - PremioBountyNumber;
				string buyin = total.ToString( Constants.BOLSA_FORMAT );
				if( Tipo == TournamentType.BOUNTY )
				{
					buyin = buyin + " + " + PremioBountyNumber.ToString( Constants.BOLSA_FORMAT ) + "(B)";
				}
				if( Liga != null )
				{
					buyin = buyin + " + " + Liga.Fee.ToString( Constants.BOLSA_FORMAT ) + "(L)";
				}
				return buyin;
			}
		}

		public int PrecioBuyinNumber { get; set; }

		[Display( Name = "$ Re-buy" )]
		[NoMap]
		public string Precio_Rebuy { get { return PrecioRebuyNumber.ToString( Constants.BOLSA_FORMAT ); } }

		public int PrecioRebuyNumber { get; set; }

		public int Entradas { get; set; }
		public int Rebuys { get; set; }

		[Display( Name = "Entradas" )]
		public string EntradasTotales
		{
			get { return ( Entradas + Rebuys ).ToString(); }
		}

		public decimal BolsaNumber { get; set; }
		[NoMap]
		public string Bolsa
		{
			get { return BolsaNumber.ToString( Constants.BOLSA_FORMAT ); }
		}

		public string Ganador { get; set; }

		public Guid GanadorId { get; set; }

		public TournamentType Tipo { get; set; }
		public string TipoString
		{
			get
			{
				return Tipo.Humanize().Transform( To.LowerCase ).Transform( To.TitleCase );
			}
		}

		public int PremioBountyNumber { get; set; }

		[NoMap]
		public Resultados Resultados { get; set; }

		[NoMap]
		[Display( Name = "Bounty" )]
		public string PremioBounty { get { return PremioBountyNumber > 0 ? PremioBountyNumber.ToString( Constants.BOLSA_FORMAT ) : "-"; } }

		[NoMap]
		public Liga Liga { get; set; }

		private string premiacion;
		public string Premiacion
		{
			get
			{
				string[] cadena = premiacion.Split( "-" );
				List<string> result = new();
				foreach(string p in cadena )
				{
					try
					{
						float f = float.Parse( p );
						result.Add( f.ToString( Constants.PRIZE_CURRENCY_FORMAT ) );
					}
					catch(Exception)
					{
						result.Add( p );
					}
				}

				return string.Join( "-", result );
			}

			set { premiacion = value; }
		}

	}

}