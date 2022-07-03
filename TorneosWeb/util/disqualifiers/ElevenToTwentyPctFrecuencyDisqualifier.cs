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
	public class ElevenToTwentyPctFrecuencyDisqualifier : IDisqualifier
	{
		private JoserrasQuery joserrasQuery;
		private ITournamentDao tournamentDao;
		private readonly ILogger<ElevenToTwentyPctFrecuencyDisqualifier> log;

		public ElevenToTwentyPctFrecuencyDisqualifier(JoserrasQuery joserrasQuery, ITournamentDao tournamentDao, ILogger<ElevenToTwentyPctFrecuencyDisqualifier> log)
		{
			this.joserrasQuery = joserrasQuery;
			this.tournamentDao = tournamentDao;
			this.log = log;
		}

		public void Disqualify(DateTime lastDate, Estadisticas estadisticas)
		{
			// Selecciona los jugadores q han jugado entre el 11 y 20% de los juegos
			int maxTorneos = tournamentDao.GetTotalTournaments();
			List<Guid> jugadoresMenosQuincePorCiento =
					estadisticas.Detalles.Where( d => d.Torneos <= maxTorneos * 0.2 && d.Torneos > maxTorneos * 0.1 ).Select( d => d.Id ).ToList();

			// Selecciona los jugadores q no han jugado en 2 meses
			List<Guid> jugadoresInactividad = new();
			foreach( DetalleJugador det in estadisticas.Detalles.ToList() )
			{
				string qu = string.Format( Queries.FindLastPlayedTournament, det.Id );
				joserrasQuery.ExecuteQuery( qu, reader =>
				{
					while( reader.Read() )
					{
						DateTime lastTourney = (DateTime)reader["fecha"];
						if( ( lastDate - lastTourney ).TotalDays > 60 )
						{
							jugadoresInactividad.Add( det.Id );
						}
					}
				} );
			}

			// Quita a los jugadores que han jugado menos del 10% de torneos y que además no han jugado en 2 meses
			if( jugadoresMenosQuincePorCiento.Count > 0 && jugadoresInactividad.Count > 0 )
			{
				log.LogDebug( "{0} jugadores entre el 11 y 20% de los {1} torneos jugados:", jugadoresMenosQuincePorCiento.Count, maxTorneos );
				log.LogDebug( jugadoresMenosQuincePorCiento.Humanize() );
				log.LogDebug( "{0} jugadores con más de 2 meses sin jugar:", jugadoresInactividad.Count );
				log.LogDebug( jugadoresInactividad.Humanize() );
				estadisticas.Detalles.RemoveAll( d => jugadoresMenosQuincePorCiento.Contains( d.Id ) && jugadoresInactividad.Contains( d.Id ) );
			}
		}

	}

}