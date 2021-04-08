using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.Properties;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class StatsServiceImpl : IStatsService
	{
		private readonly string connString;
		private JoserrasQuery joserrasQuery;
		private IReadService readService;
		private ILogger<StatsServiceImpl> log;

		public StatsServiceImpl(IConfiguration conf, IReadService readService, JoserrasQuery joserrasQuery, ILogger<StatsServiceImpl> logger)
		{
			connString = conf.GetConnectionString( Properties.Resources.joserrasDb );
			this.joserrasQuery = joserrasQuery;
			this.readService = readService;
			log = logger;
		}

		public Estadisticas GetStats()
		{
			Estadisticas estadisticas = new Estadisticas();
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();
				estadisticas.Knockouts = readService.GetAllKnockouts();
				estadisticas.Detalles = readService.GetAllDetalleJugador();

				RemoveInactivePlayers( DateTime.Now, estadisticas, conn );

				return GetStats( estadisticas, string.Empty, conn );
			}
		}

		public Estadisticas GetStats(DateTime start, DateTime end)
		{
			Estadisticas estadisticas = new Estadisticas();
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();
				estadisticas.Knockouts = readService.GetAllKnockouts( start, end );
				estadisticas.Detalles = readService.GetAllDetalleJugador( start, end );

				RemoveInactivePlayers( end, estadisticas, conn );

				return GetStats( estadisticas, QueryUtils.FormatTorneoBetween( start, end ), conn );
			}
		}

		public Estadisticas GetStats(Liga liga)
		{
			Estadisticas estadisticas = new Estadisticas();

			if(liga.Torneos.Count < 1 )
			{
				return estadisticas;
			}

			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();
				estadisticas.Knockouts = readService.GetAllKnockouts( liga );
				estadisticas.Detalles = readService.GetAllDetalleJugador( liga );

				RemoveInactivePlayers( liga.Torneos[0].FechaDate, estadisticas, conn );

				return GetStats( estadisticas, QueryUtils.FormatTorneoIdIn( liga ), conn );
			}
		}

		private void RemoveInactivePlayers(DateTime lastDate, Estadisticas estadisticas, SqlConnection conn)
		{
			// Quita los jugadores q han jugado menos del 10% de los juegos
			int maxTorneos = estadisticas.Detalles.Max( d => d.Torneos );
			List<Guid> jugadoresMenosDiezPorCiento =
					estadisticas.Detalles.Where( d => d.Torneos < maxTorneos * 0.15  ).Select( d => d.Id ).ToList();

			// Quita los jugadores q no han jugado en 3 meses
			List<Guid> jugadoresInactividad = new List<Guid>();
			foreach( DetalleJugador det in estadisticas.Detalles.ToList() )
			{
				string qu = string.Format( Queries.FindLastPlayedTournament, det.Id );
				joserrasQuery.ExecuteQuery( conn, qu, reader =>
				{
					while( reader.Read() )
					{
						DateTime lastTourney = (DateTime)reader[ "fecha" ];
						if( (lastDate - lastTourney).TotalDays > 90 )
						{
							jugadoresInactividad.Add( det.Id );
						}
					}
				} );
			}

			if( jugadoresMenosDiezPorCiento.Count > 0 && jugadoresInactividad.Count > 0 )
			{
				log.LogDebug( "{0} jugadores con menos del 15% de los {1} torneos jugados:", jugadoresMenosDiezPorCiento.Count, maxTorneos );
				log.LogDebug( jugadoresMenosDiezPorCiento.Humanize() );
				log.LogDebug( "{0} jugadores con más de 3 meses sin jugar:", jugadoresInactividad.Count );
				log.LogDebug( jugadoresInactividad.Humanize() );
				estadisticas.Detalles.RemoveAll( d => jugadoresMenosDiezPorCiento.Contains( d.Id ) && jugadoresInactividad.Contains( d.Id ) );
			}
		}

		private Estadisticas GetStats(Estadisticas estadisticas, string q, SqlConnection conn)
		{
			estadisticas.Jugadores = new SortedSet<string>( from e in estadisticas.Detalles orderby e.Nombre ascending select e.Nombre );

			estadisticas.Stats = new List<Stat>();
			if(estadisticas.Detalles.Count < 1 )
			{
				return estadisticas;
			}

			Stat joserramon = new Stat( "Joserramón", "Más Profit", "joseramon_t.png" );
			joserramon.Participantes.Add( new StatProps( estadisticas.Detalles[ 0 ].Nombre, estadisticas.Detalles[ 0 ].Profit,
				estadisticas.Detalles[ 0 ].ProfitNumber > 0 ? true : false ) );

			Stat pichon = new Stat( "Pichón", "Mayores pérdidas", "pichon_t.png" );
			decimal mayor = estadisticas.Detalles.Last().ProfitNumber;
			pichon.Participantes.AddRange( from d in estadisticas.Detalles where d.ProfitNumber == mayor select new StatProps( d.Nombre, d.Profit, d.ProfitNumber > 0 ? true : false ) );

			Stat brailovsky = new Stat( "Brailovsky", "Mayor ROI", "brailovsky_t.png" );
			DetalleJugador dj = estadisticas.Detalles.OrderByDescending( p => p.ROINumber ).First();
			brailovsky.Participantes.Add( new StatProps( dj.Nombre, dj.ROI, dj.ROINumber > 0 ? true : false ) );

			Stat victorias = new Stat( "Medalla de Oro", "Más 1ros lugares", "medalla_t.png" );
			IEnumerable<DetalleJugador> dets = estadisticas.Detalles.OrderByDescending( p => p.Victorias );
			mayor = dets.First().Victorias;
			victorias.Participantes.AddRange( from d in dets where d.Victorias == mayor select new StatProps( d.Nombre, d.Victorias.ToString() ) );

			Stat podios = new Stat( "Podium Master", "Más podios", "podium_t.png" );
			dets = estadisticas.Detalles.OrderByDescending( p => p.Podios );
			mayor = dets.First().Podios;
			podios.Participantes.AddRange( from d in dets where d.Podios == mayor select new StatProps( d.Nombre, d.Podios.ToString() ) );

			Stat coyote = new Stat( "Wile E. Coyote", "Menos podios", "coyote_t.png" );
			dets = estadisticas.Detalles.OrderBy( p => p.Podios );
			mayor = dets.First().Podios;
			coyote.Participantes.AddRange( from d in dets where d.Podios == mayor select new StatProps( d.Nombre, d.Podios.ToString() ) );

			Stat ribeyes = new Stat( "Rey del Rib-eye", "Más rebuys", "ribeye_t.png" );
			dets = estadisticas.Detalles.OrderByDescending( p => p.Rebuys );
			mayor = dets.First().Rebuys;
			ribeyes.Participantes.AddRange( from d in dets where d.Rebuys == mayor select new StatProps( d.Nombre, d.Rebuys.ToString() ) );

			Stat kos = new Stat( "Tyson", "Más KO's", "tyson_t.png" );
			dets = estadisticas.Detalles.OrderByDescending( p => p.KosNumber );
			mayor = dets.First().KosNumber;
			kos.Participantes.AddRange( from d in dets
																	where d.KosNumber == mayor
																	select new StatProps( d.Nombre, d.KosNumber.ToString( Constants.POINTS_FORMAT ) ) );

			Stat bundy = new Stat( "Al Bundy", "Más últimos lugares", "albundy_t.jpg" );
			string query = string.Format( Queries.GetBundy, q );
			Dictionary<string, int> bundies = new Dictionary<string, int>();
			joserrasQuery.ExecuteQuery( conn, query, reader =>
			{
				while( reader.Read() )
				{
					bundies.Add( reader.GetString( 0 ), reader.GetInt32( 1 ) );
				}
			} );
			mayor = bundies.Aggregate( (pair1, pair2) => pair1.Value > pair2.Value ? pair1 : pair2 ).Value;
			bundy.Participantes.AddRange( from p in bundies where p.Value == mayor select new StatProps( p.Key, p.Value.ToString() ) );

			Stat bubbleBoy = new Stat( "Eterno 4to", "Más burbujas", "bubbleboy_t.png" );
			query = string.Format( Queries.GetBurbuja, q );
			Dictionary<string, int> burbujas = new Dictionary<string, int>();
			joserrasQuery.ExecuteQuery( conn, query, reader =>
			{
				while( reader.Read() )
				{
					burbujas.Add( reader.GetString( 0 ), reader.GetInt32( 1 ) );
				}
			} );
			mayor = burbujas.Aggregate( (pair1, pair2) => pair1.Value > pair2.Value ? pair1 : pair2 ).Value;
			bubbleBoy.Participantes.AddRange( from b in burbujas where b.Value == mayor select new StatProps( b.Key, b.Value.ToString() ) );

			Stat sniper = new Stat( "Sniper", "Más KO's a un jugador", "sniper_t.png" );
			query = string.Format( Queries.GetAllKOs, q );
			List<Tuple<string, string, decimal>> tuples = new List<Tuple<string, string, decimal>>();
			joserrasQuery.ExecuteQuery( conn, query, reader =>
			{
				while( reader.Read() )
				{
					tuples.Add( new Tuple<string, string, decimal>( reader.GetString( 0 ), reader.GetString( 1 ), reader.GetDecimal( 2 ) ) );
				}
			} );
			mayor = tuples.Aggregate( (pair1, pair2) => pair1.Item3 > pair2.Item3 ? pair1 : pair2 ).Item3;
			sniper.Participantes.AddRange( from t in tuples
																		 where t.Item3 == mayor
																		 select new StatProps( t.Item1 + " a " + t.Item2, t.Item3.ToString( Constants.POINTS_FORMAT ) ) );

			Stat piedra = new Stat( "Piedra de la Victoria", "Más podios negativos", "piedra_t.png" );
			query = string.Format( Queries.GetPodiosNegativos, q );
			Dictionary<string, int> props = new Dictionary<string, int>();
			joserrasQuery.ExecuteQuery( conn, query, reader =>
			{
				while( reader.Read() )
				{
					props[ reader.GetString( 0 ) ] = reader.GetInt32( 1 );
				}
			} );
			if( props.Count > 0 )
			{
				mayor = props.Aggregate( (pair1, pair2) => pair1.Value > pair2.Value ? pair1 : pair2 ).Value;
				piedra.Participantes.AddRange( from b in props where b.Value == mayor select new StatProps( b.Key, b.Value.ToString() ) ); 
			}

			estadisticas.Stats.Add( joserramon );
			estadisticas.Stats.Add( brailovsky );
			estadisticas.Stats.Add( pichon );
			estadisticas.Stats.Add( victorias );
			estadisticas.Stats.Add( podios );
			estadisticas.Stats.Add( coyote );
			estadisticas.Stats.Add( bubbleBoy );
			estadisticas.Stats.Add( ribeyes );
			estadisticas.Stats.Add( kos );
			estadisticas.Stats.Add( sniper );
			estadisticas.Stats.Add( piedra );
			estadisticas.Stats.Add( bundy );

			return estadisticas;
		}

	}

}