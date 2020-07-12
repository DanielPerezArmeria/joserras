using System;

namespace TorneosWeb.domain.models
{
	public class Posicion
	{
		public Guid JugadorId { get; set; }
		public string Nombre { get; set; }
		public int Lugar { get; set; }
		public string Premio { get; set; }
		public string Podio { get; set; }
		public int Rebuys { get; set; }
		public string Burbuja { get; set; }
		public int Knockouts { get; set; }
		public string Profit { get; set; }
		public int ProfitNumber { get; set; }

	}

}