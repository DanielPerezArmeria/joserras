using System;

namespace TorneosWeb.domain.dto.ligas
{
	public class LigaDTO
	{
		public Guid Id { get; set; }

		public string Nombre { get; set; }

		public DateTime FechaCreada { get; set; }

		public DateTime FechaCerrada { get; set; }

		public bool Abierta { get; set; }

		public string Puntaje { get; set; }
	}

}