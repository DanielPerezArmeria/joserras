using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.dao;

namespace TorneosWeb.service.impl
{
	public class JugadorService :IJugadorService
	{
		private ITournamentDao tournamentDao;
		private IReadService readService;

		public JugadorService(IReadService readService, ITournamentDao tournamentDao)
		{
			this.readService = readService;
			this.tournamentDao = tournamentDao;
		}

		public List<Posicion> GetAllPosicionesByJugador(Guid jugadorId)
		{
			List<Posicion> posiciones = tournamentDao.GetAllPosicionesByJugador( jugadorId );
			List<Torneo> torneos = readService.GetAllTorneos();
			foreach( Posicion p in posiciones )
			{
				p.Torneo = torneos.First( t => t.Id.Equals( p.TorneoId ) );
			}
			return posiciones;
		}

		public List<Jugador> GetAllJugadores()
		{
			return tournamentDao.GetAllJugadores();
		}

	}

}