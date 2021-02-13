﻿using CsvHelper.Configuration.Attributes;
using System;

namespace TorneosWeb.domain.dto
{
	public class ResultadosDTO
	{
		public Guid TorneoId { get; set; }
		public Guid JugadorId { get; set; }
		public string Jugador { get; set; }
		public int Posicion { get; set; }

		[Default( 0 )]
		public int Rebuys { get; set; }

		[Default( false )]
		public bool Podio { get; set; }

		[Default( "0" )]
		public string Premio { get; set; }

		[Default( false )]
		public bool Burbuja { get; set; }

		[Default(true)]
		public bool Puntualidad { get; set; }

		public decimal Kos { get; set; }

		[Default(false)]
		public bool Nuevo { get; set; }
	}

}