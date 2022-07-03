using Humanizer;
using Joserras.Commons.Db;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.Properties;

namespace TorneosWeb.util.disqualifiers
{
	public class SixMonthInactiveDisqualifier : IDisqualifier
	{
		private JoserrasQuery joserrasQuery;
		private readonly ILogger<SixMonthInactiveDisqualifier> log;

		public SixMonthInactiveDisqualifier(JoserrasQuery joserrasQuery, ILogger<SixMonthInactiveDisqualifier> log)
		{
			this.joserrasQuery = joserrasQuery;
			this.log = log;
		}

		public void Disqualify(DateTime lastDate, Estadisticas estadisticas)
		{
			List<Guid> jugadoresInactividad = new();

			foreach( DetalleJugador det in estadisticas.Detalles.ToList() )
			{
				string qu = string.Format( Queries.FindLastPlayedTournament, det.Id );
				joserrasQuery.ExecuteQuery( qu, reader =>
				{
					while( reader.Read() )
					{
						DateTime lastTourney = (DateTime)reader["fecha"];
						if( ( lastDate - lastTourney ).TotalDays > 180 )
						{
							jugadoresInactividad.Add( det.Id );
						}
					}
				} );
			}

			log.LogDebug( "{0} jugadores con más de 6 meses sin jugar:", jugadoresInactividad.Count );
			log.LogDebug( jugadoresInactividad.Humanize() );
			estadisticas.Detalles.RemoveAll( d => jugadoresInactividad.Contains( d.Id ) );
		}

	}

}