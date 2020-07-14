using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TorneosWeb.domain.models;
using TorneosWeb.util;
using System.Linq;

namespace TorneosWeb.service.impl
{
	public class ReadServiceImpl : IReadService
	{
		private string connString;
		private IMapper mapper;
		private JoserrasQuery joserrasQuery;

		public ReadServiceImpl(IConfiguration conf, IMapper mapper, JoserrasQuery joserrasQuery)
		{
			connString = conf.GetConnectionString( Properties.Resources.joserrasDb);
			this.mapper = mapper;
			this.joserrasQuery = joserrasQuery;
		}

		public List<Jugador> GetAllJugadores()
		{
			List<Jugador> jugadores = new List<Jugador>();
			string query = "select * from jugadores";
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Jugador jugador = new Jugador();
					jugador.Id = reader.GetGuid( 0 );
					jugador.Nombre = reader.GetString( 1 );
					jugadores.Add( jugador );
				}
			} );

			return jugadores;
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			SortedList<string, Dictionary<string, Knockouts>> knockouts = new SortedList<string, Dictionary<string, Knockouts>>();
			string query = Properties.Queries.GetAllKOs;

			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Knockouts ko = mapper.Map<SqlDataReader, Knockouts>( reader );
					if( !knockouts.ContainsKey( ko.Nombre ) )
					{
						knockouts.Add( ko.Nombre, new Dictionary<string, Knockouts>() );
					}
					knockouts[ ko.Nombre ].Add( ko.Eliminado, ko );
				}
			} );

			return knockouts;
		}

		public List<Torneo> GetAllTorneos()
		{
			List<Torneo> torneos = new List<Torneo>();

			joserrasQuery.ExecuteQuery( Properties.Queries.GetAllTorneos, reader =>
			{
				while( reader.Read() )
				{
					Torneo torneo = mapper.Map<SqlDataReader, Torneo>( reader );
					torneos.Add( torneo );
				}
			} );

			return torneos;
		}

		public DetalleTorneo GetDetalleTorneo(Guid torneoId)
		{
			DetalleTorneo detalleTorneo = new DetalleTorneo();

			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();
				string query = string.Format( "select * from torneos where id = '{0}'", torneoId );
				Torneo torneo = joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					reader.Read();
					return mapper.Map<SqlDataReader, Torneo>( reader );
				} );
				detalleTorneo.Torneo = torneo;

				query = string.Format("select dt.*, j.nombre from detalletorneos dt, jugadores j "
						+ "where dt.torneo_id = '{0}' and dt.jugador_id = j.id order by dt.posicion", torneoId);
				detalleTorneo.Posiciones = new List<Posicion>();
				detalleTorneo.Jugadores = new SortedSet<string>();
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						Posicion posicion = mapper.Map<SqlDataReader, Posicion>( reader );
						int premio = 0;
						try
						{
							premio = int.Parse( posicion.Premio, System.Globalization.NumberStyles.Currency );
						}
						catch( FormatException ) { }
						posicion.ProfitNumber = premio - (int.Parse( detalleTorneo.Torneo.Precio_Buyin, System.Globalization.NumberStyles.Currency )
								+ (posicion.Rebuys * int.Parse( detalleTorneo.Torneo.Precio_Rebuy, System.Globalization.NumberStyles.Currency )));
						posicion.Profit = (posicion.ProfitNumber).ToString("$###,###");
						detalleTorneo.Posiciones.Add( posicion );
						detalleTorneo.Jugadores.Add( posicion.Nombre );
					}
				} );
			}

			detalleTorneo.Knockouts = GetKnockoutsByTournamentId( torneoId );

			foreach( Posicion position in detalleTorneo.Posiciones )
			{
				if( detalleTorneo.Knockouts.ContainsKey( position.Nombre ) )
				{
					foreach(Knockouts ko in detalleTorneo.Knockouts[ position.Nombre ].Values )
					{
						position.Knockouts += ko.Eliminaciones;
					}
				}
			}

			return detalleTorneo;
		}

		public DetalleJugador GetDetalleJugador(Guid playerId)
		{
			DetalleJugador detalle = null;
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();

				string query = string.Format( Properties.Queries.GetDetalleJugador, playerId );
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					reader.Read();
					detalle = mapper.Map<DetalleJugador>( reader );
				} );
			}

			detalle.Knockouts = GetKnockoutsByPlayer( playerId );

			return detalle;
		}

		public DetalleJugador GetDetalleJugador(string nombre)
		{
			throw new NotImplementedException();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockoutsByTournamentId(Guid torneoId)
		{
			SortedList<string, Dictionary<string, Knockouts>> knockouts = new SortedList<string, Dictionary<string,Knockouts>>();
			string query = string.Format( "select j.nombre, elim.nombre as eliminado, sum(e.eliminaciones) as eliminaciones from eliminaciones e, jugadores j, jugadores elim"
					+ " where torneo_id = '{0}' and e.jugador_id = j.id and e.eliminado_id = elim.id group by j.nombre, elim.nombre", torneoId );

			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Knockouts ko = mapper.Map<SqlDataReader, Knockouts>( reader );
					if( !knockouts.ContainsKey( ko.Nombre ) )
					{
						knockouts.Add( ko.Nombre, new Dictionary<string, Knockouts>() );
					}
					knockouts[ ko.Nombre ].Add(ko.Eliminado, ko );
				}
			} );

			return knockouts;
		}

		public List<Knockouts> GetKnockoutsByPlayer(Guid playerId)
		{
			List<Knockouts> kos = new List<Knockouts>();
			string query = string.Format( Properties.Queries.GetKnockoutsByPlayer, playerId );

			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					kos.Add( mapper.Map<SqlDataReader, Knockouts>( reader ) );
				}
			} );

			return kos;
		}

		public List<DetalleJugador> GetDetalleJugador()
		{
			List<DetalleJugador> detalles = new List<DetalleJugador>();
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();

				string query = string.Format( Properties.Queries.GetStats );
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						detalles.Add( mapper.Map<DetalleJugador>( reader ) ); 
					}
				} );
			}

			return detalles;
		}

		public Estadisticas GetStats()
		{
			Estadisticas estadisticas = new Estadisticas();

			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();

				estadisticas.Knockouts = GetAllKnockouts();
				estadisticas.Detalles = GetDetalleJugador();
				estadisticas.Jugadores = new SortedSet<string>( from e in estadisticas.Detalles orderby e.Nombre ascending select e.Nombre );

				estadisticas.Stats = new List<Stat>();

				Stat joserramon = new Stat( "Joserramón", "Más Profit", "joseramon_t.png" );
				joserramon.Participantes.Add( new StatProps( estadisticas.Detalles[ 0 ].Nombre, estadisticas.Detalles[ 0 ].Profit,
					estadisticas.Detalles[ 0 ].ProfitNumber > 0 ? true : false ) );

				Stat pichon = new Stat( "El Pichón", "Mayores pérdidas", "pichon_t.png" );
				int mayor = estadisticas.Detalles.Last().ProfitNumber;
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

				Stat kos = new Stat( "Tyson", "Más eliminaciones", "tyson_t.png" );
				dets = estadisticas.Detalles.OrderByDescending( p => p.Kos );
				mayor = dets.First().Kos;
				kos.Participantes.AddRange( from d in dets where d.Kos == mayor select new StatProps( d.Nombre, d.Kos.ToString() ) );

				Stat bundy = new Stat( "Al Bundy", "Más últimos lugares", "albundy_t.jpg" );
				string query = Properties.Queries.GetBundy;
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

				Stat bubbleBoy = new Stat("Eterno 4to", "Más burbujas", "bubbleboy_t.png");
				query = Properties.Queries.GetBundy;
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

				Stat sniper = new Stat( "Sniper", "Más eliminaciones a un jugador", "sniper_t.png" );
				query = Properties.Queries.GetAllKOs;
				List<Tuple<string, string, int>> tuples = new List<Tuple<string, string, int>>();
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						tuples.Add( new Tuple<string, string, int>( reader.GetString( 0 ), reader.GetString( 1 ), reader.GetInt32( 2 ) ) );
					}
				} );
				mayor = tuples.Aggregate( (pair1, pair2) => pair1.Item3 > pair2.Item3 ? pair1 : pair2 ).Item3;
				sniper.Participantes.AddRange( from t in tuples where t.Item3 == mayor select new StatProps( t.Item1 + " a " + t.Item2, t.Item3.ToString() ) );

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
				estadisticas.Stats.Add( bundy );
			}

			return estadisticas;
		}

	}

}