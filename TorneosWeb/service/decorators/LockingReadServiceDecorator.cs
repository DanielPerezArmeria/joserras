using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.decorators
{
	public class LockingReadServiceDecorator : IReadService
	{
		private readonly IReadService wrapped;
		private readonly object locker = new();

		public LockingReadServiceDecorator(IReadService wrapped)
		{
			this.wrapped = wrapped;
		}

		public DetalleJugador FindDetalleJugador(Guid id)
		{
			lock (locker)
			{
				return wrapped.FindDetalleJugador( id );
			}
		}

		public DetalleJugador FindDetalleJugador(string nombre)
		{
			lock (locker)
			{
				return wrapped.FindDetalleJugador( nombre );
			}
		}

		public Resultados FindResultadosTorneo(Guid id)
		{
			lock (locker)
			{
				return wrapped.FindResultadosTorneo( id );
			}
		}

		public Torneo FindTorneoByFecha(DateTime fecha)
		{
			lock (locker)
			{
				return wrapped.FindTorneoByFecha( fecha );
			}
		}

		public List<DetalleJugador> GetAllDetalleJugador()
		{
			lock (locker)
			{
				return wrapped.GetAllDetalleJugador();
			}
		}

		public List<DetalleJugador> GetAllDetalleJugador(DateTime start, DateTime end)
		{
			lock (locker)
			{
				return wrapped.GetAllDetalleJugador( start, end );
			}
		}

		public List<DetalleJugador> GetAllDetalleJugador(Liga liga)
		{
			lock (locker)
			{
				return wrapped.GetAllDetalleJugador( liga );
			}
		}

		public List<Jugador> GetAllJugadores()
		{
			lock (locker)
			{
				return wrapped.GetAllJugadores();
			}
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			lock (locker)
			{
				return wrapped.GetAllKnockouts();
			}
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(DateTime start, DateTime end)
		{
			lock (locker)
			{
				return wrapped.GetAllKnockouts( start, end );
			}
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(Liga liga)
		{
			lock (locker)
			{
				return wrapped.GetAllKnockouts( liga );
			}
		}

		public List<Torneo> GetAllTorneos()
		{
			lock (locker)
			{
				return wrapped.GetAllTorneos();
			}
		}

		public List<Knockouts> GetKnockoutsByPlayer(Guid playerId)
		{
			lock (locker)
			{
				return wrapped.GetKnockoutsByPlayer( playerId );
			}
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockoutsByTournamentId(Guid torneoId)
		{
			lock (locker)
			{
				return wrapped.GetKnockoutsByTournamentId( torneoId );
			}
		}

		public List<Knockouts> GetTournamentKOList(Guid torneoId)
		{
			lock (locker)
			{
				return wrapped.GetTournamentKOList( torneoId );
			}
		}

	}

}