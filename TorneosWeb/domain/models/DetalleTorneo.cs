using System.Collections.Generic;

namespace TorneosWeb.domain.models
{
	public class DetalleTorneo
	{
		public List<Posicion> Posiciones { get; set; }
		public Torneo Torneo { get; set; }
		public SortedList<string, Dictionary<string, Knockouts>> Knockouts { get; set; }
		public SortedSet<string> Jugadores { get; set; }

	}

}