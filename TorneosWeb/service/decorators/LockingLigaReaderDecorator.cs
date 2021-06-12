using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.decorators
{
	public class LockingLigaReaderDecorator : ILigaReader
	{
		private ILigaReader wrapped;
		private object locker = new();

		public LockingLigaReaderDecorator(ILigaReader wrapped)
		{
			this.wrapped = wrapped;
		}

		public Liga FindLigaByNombre(string nombre)
		{
			lock (locker)
			{
				return wrapped.FindLigaByNombre( nombre );
			}
		}

		public List<Liga> GetAllLigas()
		{
			lock (locker)
			{
				return wrapped.GetAllLigas();
			}
		}

		public Liga GetCurrentLiga()
		{
			lock (locker)
			{
				return wrapped.GetCurrentLiga();
			}
		}

		public Liga GetLigaByTorneoId(Guid torneoId)
		{
			lock (locker)
			{
				return wrapped.GetLigaByTorneoId( torneoId );
			}
		}

		public List<Standing> GetStandings(Liga liga)
		{
			lock (locker)
			{
				return wrapped.GetStandings( liga );
			}
		}

		public List<Standing> GetStandings(Liga liga, Torneo torneo)
		{
			lock (locker)
			{
				return wrapped.GetStandings( liga, torneo );
			}
		}

	}

}