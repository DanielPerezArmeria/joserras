using System;
using System.Collections.Generic;
using TorneosWeb.util.automapper;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.domain.models.ligas
{
	public class Liga
	{
		public Guid Id { get; set; }
		public string Nombre { get; set; }

		private string puntaje;
		public string Puntaje
		{
			get { return puntaje; }
			set
			{
				puntaje = value;
				PointRules = PointRule.Build( puntaje );
			}
		}

		public bool Abierta { get; set; }

		[NoMap]
		public string FechaInicio { get; set; }
		public DateTime FechaInicioDate { get; set; }

		[NoMap]
		public string FechaCierre { get; set; }
		public DateTime? FechaCierreDate { get; set; }

		public int Fee { get; set; }

		[NoMap]
		public Dictionary<string, PointRule> PointRules { get; set; }

	}

}