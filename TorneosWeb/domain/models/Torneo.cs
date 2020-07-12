using System;
using System.ComponentModel.DataAnnotations;

namespace TorneosWeb.domain.models
{
	public class Torneo
	{
		public Guid Id { get; set; }
		public string Fecha { get; set; }
		[Display( Name = "$ Buyin" )]
		public string Precio_Buyin { get; set; }
		[Display( Name = "$ Re-buy")]
		public string Precio_Rebuy { get; set; }
		public int Entradas { get; set; }
		public int Rebuys { get; set; }
		public string Bolsa { get; set; }
		public string Ganador { get; set; }
		public Guid GanadorId { get; set; }

	}

}