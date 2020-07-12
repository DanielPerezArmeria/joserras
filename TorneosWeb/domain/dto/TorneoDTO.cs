using System;

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
	}

}