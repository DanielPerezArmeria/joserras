using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service
{
	public interface IReadService
	{
		List<Torneo> GetAllTorneos();

		List<Jugador> GetAllJugadores();

		Resultados FindResultadosTorneo(Guid id);

		DetalleJugador FindDetalleJugador(Guid id);

		DetalleJugador FindDetalleJugador(string nombre);

		List<DetalleJugador> GetAllDetalleJugador();

		SortedList<string, Dictionary<string,Knockouts>> GetKnockoutsByTournamentId(Guid torneoId);

		List<Knockouts> GetKnockoutsByPlayer(Guid playerId);

		SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts();

		Torneo FindTorneoByFecha(DateTime fecha);

	}

}