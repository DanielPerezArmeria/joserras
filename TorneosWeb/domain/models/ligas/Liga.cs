using System;
using TorneosWeb.util.automapper;

namespace TorneosWeb.domain.models.ligas
{
	public class Liga
	{
		public Guid Id { get; set; }
		public string Nombre { get; set; }
		public string Puntaje { get; set; }
		public bool Abierta { get; set; }

		[NoMap]
		public string FechaInicio { get; set; }
		public DateTime FechaInicioDate { get; set; }

		[NoMap]
		public string FechaCierre { get; set; }
		public DateTime? FechaCierreDate { get; set; }

	}

}