using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service
{
	public interface IReadService
	{
		List<Torneo> GetAllTorneos();

		List<Jugador> GetAllJugadores();

		DetalleTorneo GetDetalleTorneo(Guid id);

		DetalleJugador GetDetalleJugador(Guid id);

		DetalleJugador GetDetalleJugador(string nombre);

		List<DetalleJugador> GetDetalleJugador();

		SortedList<string, Dictionary<string,Knockouts>> GetKnockouts(Guid torneoId);

		List<Knockouts> GetKnockoutsByPlayer(Guid playerId);

		SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts();

		Estadisticas GetStats();

	}

}