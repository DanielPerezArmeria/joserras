using System;

namespace TorneosWeb.domain.dto
{
	public class DetalleTorneoDTO
	{
		public Guid TorneoId { get; set; }
		public Guid JugadorId { get; set; }
		public string Jugador { get; set; }
		public int Rebuys { get; set; }
		public int Posicion { get; set; }
		public bool Podio { get; set; }
		public int Premio { get; set; }
		public bool Burbuja { get; set; }
	}

}