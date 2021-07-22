using Joserras.Commons.Domain;
using System;

namespace Joserras.Commons.Dto
{
	public class TorneoDTO
	{
		public TorneoDTO()
		{
			Tipo = TournamentType.NORMAL;
		}

		public Guid Id { get; set; }
		public DateTime Fecha { get; set; }
		public int PrecioBuyin { get; set; }
		public int PrecioRebuy { get; set; }
		public int Entradas { get; set; }
		public int Rebuys { get; set; }
		public Bolsa Bolsa { get; set; }

		public int PrecioBounty { get; set; }

		public TournamentType Tipo { get; set; }

		public bool Liga { get; set; }

		public string Premiacion { get; set; }
	}

}