using System;

namespace Joserras.Commons.Dto
{
	public class ResultadosDTO
	{
		public ResultadosDTO()
		{
			Premio = "0";
			Puntualidad = true;
			Rebuys = 0;
		}

		public Guid TorneoId { get; set; }
		public Guid JugadorId { get; set; }
		public string Jugador { get; set; }
		public int Posicion { get; set; }

		public int Rebuys { get; set; }

		public bool Podio { get; set; }

		public string Premio { get; set; }

		public bool Burbuja { get; set; }

		public bool Puntualidad { get; set; }

		public decimal Kos { get; set; }

		public bool Nuevo { get; set; }
	}

}