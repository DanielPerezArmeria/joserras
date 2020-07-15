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

	}

}