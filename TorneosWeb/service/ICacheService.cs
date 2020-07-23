using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service
{
	public interface ICacheService
	{
		void Clear();

		List<Jugador> GetAllJugadores { get; }

		SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts { get; }

		List<Torneo> GetAllTorneos { get; }

		Dictionary<Guid, DetalleTorneo> GetDetalleTorneo { get; }

		Dictionary<Guid, DetalleJugador> GetDetalleJugadorById { get; }

		Dictionary<Guid,SortedList<string, Dictionary<string, Knockouts>>> GetKnockoutsByTournament { get; }

		Dictionary<Guid, List<Knockouts>> GetKnockoutsByPlayer { get; }
		
		List<DetalleJugador> GetDetalleJugador { get; }

		Estadisticas GetStats { get; set; }
	}

}