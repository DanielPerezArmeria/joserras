using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service
{
	public interface IJugadorService
  {
    List<Posicion> GetAllPosicionesByJugador(Guid jugadorId);

    List<Jugador> GetAllJugadores();
  }

}