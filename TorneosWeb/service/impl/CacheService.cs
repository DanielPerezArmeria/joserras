using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service.impl
{
	public class CacheService : ICacheService
	{
		private readonly List<Jugador> getAllJugadores = new List<Jugador>();
		public List<Jugador> GetAllJugadores => getAllJugadores;

		private SortedList<string, Dictionary<string, Knockouts>> getAllKnockouts =
				new SortedList<string, Dictionary<string, Knockouts>>();
		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts => getAllKnockouts;

		private List<Torneo> getAllTorneos = new List<Torneo>();
		public List<Torneo> GetAllTorneos => getAllTorneos;

		private Dictionary<Guid, Resultados> getDetalleTorneo = new Dictionary<Guid, Resultados>();
		public Dictionary<Guid, Resultados> GetDetalleTorneo => getDetalleTorneo;

		private Dictionary<Guid, DetalleJugador> getDetalleJugadorById = new Dictionary<Guid, DetalleJugador>();
		public Dictionary<Guid, DetalleJugador> GetDetalleJugadorById => getDetalleJugadorById;

		private Dictionary<Guid, SortedList<string, Dictionary<string, Knockouts>>> getKnockoutsByTournament =
				new Dictionary<Guid, SortedList<string, Dictionary<string, Knockouts>>>();
		public Dictionary<Guid, SortedList<string, Dictionary<string, Knockouts>>> GetKnockoutsByTournament => getKnockoutsByTournament;

		private Dictionary<Guid, List<Knockouts>> getKnockoutsByPlayer = new Dictionary<Guid, List<Knockouts>>();
		public Dictionary<Guid, List<Knockouts>> GetKnockoutsByPlayer => getKnockoutsByPlayer;

		private List<DetalleJugador> getDetalleJugador = new List<DetalleJugador>();
		public List<DetalleJugador> GetDetalleJugador => getDetalleJugador;

		private Estadisticas getStats = null;
		public Estadisticas GetStats { get => getStats; set => getStats = value; }

		public void Clear()
		{
			GetAllJugadores.Clear();
			GetAllKnockouts.Clear();
			GetAllTorneos.Clear();
			GetDetalleTorneo.Clear();
			GetDetalleJugadorById.Clear();
			GetKnockoutsByTournament.Clear();
			GetKnockoutsByPlayer.Clear();
			GetDetalleJugador.Clear();
			GetStats = null;
		}

	}

}