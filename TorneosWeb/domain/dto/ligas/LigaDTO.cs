﻿using System;

namespace TorneosWeb.domain.dto.ligas
{
	public class LigaDTO
	{
		public Guid Id { get; set; }

		public string Nombre { get; set; }

		public DateTime FechaInicio { get; set; }

		public DateTime FechaCierre { get; set; }

		public bool Abierta { get; set; }

		public string Puntaje { get; set; }

		public int Fee { get; set; }

		public string Premiacion { get; set; }
	}

}