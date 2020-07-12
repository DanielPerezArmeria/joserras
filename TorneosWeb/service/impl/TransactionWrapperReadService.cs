using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.exception;

namespace TorneosWeb.service.impl
{
	public class TransactionWrapperReadService : IReadService
	{
		private IReadService wrapped;

		public TransactionWrapperReadService(IReadService readService)
		{
			this.wrapped = readService;
		}

		public List<Jugador> GetAllJugadores()
		{
			try
			{
				return wrapped.GetAllJugadores();
			}
			catch( Exception e )
			{
				throw new JoserrasException( e );
			}
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			try
			{
				return wrapped.GetAllKnockouts();
			}
			catch( Exception e )
			{
				throw new JoserrasException( e );
			}
		}

		public List<Torneo> GetAllTorneos()
		{
			try
			{
				return wrapped.GetAllTorneos();
			}
			catch( Exception e)
			{
				throw new JoserrasException(e);
			}
		}

		public DetalleTorneo GetDetalleTorneo(Guid id)
		{
			try
			{
				return wrapped.GetDetalleTorneo( id );
			}
			catch( Exception e )
			{
				throw new JoserrasException( e );
			}
		}

		public DetalleJugador GetDetalleJugador(Guid id)
		{
			try
			{
				return wrapped.GetDetalleJugador( id );
			}
			catch( Exception e )
			{
				throw new JoserrasException( e );
			}
		}

		public DetalleJugador GetDetalleJugador(string nombre)
		{
			throw new NotImplementedException();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockouts(Guid torneoId)
		{
			try
			{
				return wrapped.GetKnockouts( torneoId );
			}
			catch( Exception e )
			{
				throw new JoserrasException( e );
			}
		}

		public List<Knockouts> GetKnockoutsByPlayer(Guid playerId)
		{
			try
			{
				return wrapped.GetKnockoutsByPlayer( playerId );
			}
			catch(Exception e )
			{
				throw new JoserrasException( e );
			}
		}

		public List<DetalleJugador> GetDetalleJugador()
		{
			try
			{
				return wrapped.GetDetalleJugador();
			}
			catch( Exception e )
			{
				throw new JoserrasException( e );
			}
		}

		public Estadisticas GetStats()
		{
			try
			{
				return wrapped.GetStats();
			}
			catch( Exception e )
			{
				throw new JoserrasException( e );
			}
		}
	}

}