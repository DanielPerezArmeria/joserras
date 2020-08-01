using CsvHelper.Configuration.Attributes;
using System;
using TorneosWeb.domain.models;

namespace TorneosWeb.domain.dto
{
	public class TorneoDTO
	{
		public Guid Id { get; set; }
		public DateTime Fecha { get; set; }
		public int PrecioBuyin { get; set; }
		public int PrecioRebuy { get; set; }
		public int Entradas { get; set; }
		public int Rebuys { get; set; }
		public int Bolsa { get; set; }

		[Default( 0 )]
		public int PrecioBounty { get; set; }

		[Default(TournamentType.NORMAL)]
		public TournamentType Tipo { get; set; }

		public string Liga { get; set; }
		public int LigaFee { get; set; }
		public string LigaPuntaje { get; set; }

		[Default(false)]
		public bool CerrarLiga { get; set; }
	}

}