using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.decorators
{
	public class LockingStatsServiceDecorator : IStatsService
	{
		private IStatsService wrapped;
		private object locker = new();

		public LockingStatsServiceDecorator(IStatsService wrapped)
		{
			this.wrapped = wrapped;
		}

		public Estadisticas GetStats()
		{
			lock (locker)
			{
				return wrapped.GetStats();
			}
		}

		public Estadisticas GetStats(DateTime start, DateTime end)
		{
			lock (locker)
			{
				return wrapped.GetStats( start, end );
			}
		}

		public Estadisticas GetStats(Liga liga)
		{
			lock (locker)
			{
				return wrapped.GetStats( liga );
			}
		}

	}

}