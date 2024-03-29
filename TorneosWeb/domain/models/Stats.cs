﻿using System.Collections.Generic;

namespace TorneosWeb.domain.models
{
	public class Estadisticas
	{
		public Estadisticas()
		{
			Knockouts = new SortedList<string, Dictionary<string, Knockouts>>();
			Stats = new List<Stat>();
			Detalles = new List<DetalleJugador>();
			Jugadores = new SortedSet<string>();
		}

		public SortedList<string, Dictionary<string, Knockouts>> Knockouts { get; set; }
		public List<Stat> Stats { get; set; }
		public SortedSet<string> Jugadores { get; set; }

		private List<DetalleJugador> detalles;
		public List<DetalleJugador> Detalles
		{
			get { return detalles; }
			set
			{
				detalles = value;
				if( detalles != null )
				{
					detalles.Sort(); 
				}
			}
		}

	}

	public class Stat
	{
		public string Titulo { get; set; }
		public string Imagen { get; set; }
		public string Descripcion { get; set; }
		public string Valor { get; set; }
		public int Level { get; set; }

		private List<StatProps> participantes;
		public List<StatProps> Participantes { get { return participantes; } }

		public Stat() { participantes = new List<StatProps>(); }

		public Stat(string titulo, string desc, string imagen)
		{
			Titulo = titulo;
			Descripcion = desc;
			Imagen = imagen;
			Level = 0;
			participantes = new List<StatProps>();
		}

		public Stat(string titulo, string desc, string imagen, int level)
		{
			Titulo = titulo;
			Descripcion = desc;
			Imagen = imagen;
			Level = level;
			participantes = new List<StatProps>();
		}

	}

}