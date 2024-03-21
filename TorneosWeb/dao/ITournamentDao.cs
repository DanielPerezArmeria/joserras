using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.dao
{
	public interface ITournamentDao
	{
		int GetTotalTournaments();

		Torneo GetTorneo(Guid torneoId);

		List<Posicion> GetPosicionesByTorneo(Guid torneoId);

		List<Posicion> GetAllPosicionesByJugador(Guid jugadorId);

		List<Torneo> GetAllTorneos();

		Torneo FindTorneoByFecha(DateTime fecha);

		List<Jugador> GetAllJugadores();
	}

}