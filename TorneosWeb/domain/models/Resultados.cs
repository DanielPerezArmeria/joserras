﻿using System.Collections.Generic;

namespace TorneosWeb.domain.models
{
	public class Resultados
	{
		public Resultados()
		{
			Knockouts = new();
			KnockoutList = new();
		}

		public Torneo Torneo { get; set; }
		public SortedList<string, Dictionary<string, Knockouts>> Knockouts { get; set; }
		public List<Knockouts> KnockoutList { get; set; }
		public SortedSet<string> Jugadores { get; set; }

		private List<Posicion> posiciones;
		public List<Posicion> Posiciones
		{
			get { return posiciones; }
			set
			{
				posiciones = value;
				if( posiciones != null )
				{
					posiciones.Sort(); 
				}
			}
		}

	}

}