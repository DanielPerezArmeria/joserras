using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service
{
	public interface IReadService
	{
		List<Torneo> GetAllTorneos();
		Torneo FindTorneoByFecha(DateTime fecha);

		List<Jugador> GetAllJugadores();

		Resultados FindResultadosTorneo(Guid id);

		DetalleJugador FindDetalleJugador(Guid id);

		DetalleJugador FindDetalleJugador(string nombre);

		List<DetalleJugador> GetAllDetalleJugador();
		List<DetalleJugador> GetAllDetalleJugador(DateTime start, DateTime end);
		List<DetalleJugador> GetAllDetalleJugador(Liga liga);

		SortedList<string, Dictionary<string,Knockouts>> GetKnockoutsByTournamentId(Guid torneoId);

		List<Knockouts> GetKnockoutsByPlayer(Guid playerId);

		SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts();
		SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(DateTime start, DateTime end);
		SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(Liga liga);

	}

}