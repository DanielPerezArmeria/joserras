using Humanizer;
using Joserras.Commons.Db;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.models;
using TorneosWeb.Properties;

namespace TorneosWeb.util.disqualifiers
{
	public class LessThanTenPctDisqualifier : IDisqualifier
	{
		private JoserrasQuery joserrasQuery;
		private ITournamentDao tournamentDao;
		private readonly ILogger<LessThanTenPctDisqualifier> log;

		public LessThanTenPctDisqualifier(JoserrasQuery joserrasQuery, ITournamentDao tournamentDao, ILogger<LessThanTenPctDisqualifier> log)
		{
			this.joserrasQuery = joserrasQuery;
			this.tournamentDao = tournamentDao;
			this.log = log;
		}

		public IList<Guid> Disqualify(DateTime lastDate, Estadisticas estadisticas)
		{
			// Selecciona los jugadores q han jugado menos del 10% de los torneos
			int maxTorneos = tournamentDao.GetTotalTournaments();
			List<Guid> jugadoresMenosDiezPorCiento =
					estadisticas.Detalles.Where( d => d.Torneos <= maxTorneos * 0.1 ).Select( d => d.Id ).ToList();

			// Selecciona los jugadores q no han jugado en 1 meses
			List<Guid> jugadoresInactividad = new();
			foreach( DetalleJugador det in estadisticas.Detalles.ToList() )
			{
				string qu = string.Format( Queries.FindLastPlayedTournament, det.Id );
				joserrasQuery.ExecuteQuery( qu, reader =>
				{
					while( reader.Read() )
					{
						DateTime lastTourney = (DateTime)reader["fecha"];
						if( ( lastDate - lastTourney ).TotalDays > 30 )
						{
							jugadoresInactividad.Add( det.Id );
						}
					}
				} );
			}

			// Quita a los jugadores que han jugado menos del 10% de torneos y que además no han jugado en 1 mes
			if( jugadoresMenosDiezPorCiento.Count > 0 && jugadoresInactividad.Count > 0 )
			{
				log.LogDebug( "{0} jugadores con menos del 10% de los {1} torneos jugados:", jugadoresMenosDiezPorCiento.Count, maxTorneos );
				log.LogDebug( jugadoresMenosDiezPorCiento.Humanize() );
				log.LogDebug( "{0} jugadores con más de 1 mes sin jugar:", jugadoresInactividad.Count );
				log.LogDebug( jugadoresInactividad.Humanize() );
			}

			return jugadoresMenosDiezPorCiento.Intersect( jugadoresInactividad ).ToList();
		}

	}

}