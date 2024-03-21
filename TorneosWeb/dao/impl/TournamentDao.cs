using AutoMapper;
using Joserras.Commons.Db;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TorneosWeb.domain.models;

namespace TorneosWeb.dao.impl
{
	public class TournamentDao : ITournamentDao
	{
		private JoserrasQuery joserrasQuery;
		private IMapper mapper;
		private ILogger<TournamentDao> log;

		public TournamentDao(JoserrasQuery joserrasQuery, IMapper mapper, ILogger<TournamentDao> log)
		{
			this.joserrasQuery = joserrasQuery;
			this.mapper = mapper;
			this.log = log;
		}

		public List<Posicion> GetPosicionesByTorneo(Guid torneoId)
		{
			List<Posicion> posiciones = new List<Posicion>();

			string query = @"select dt.*, j.nombre from resultados dt, jugadores j where dt.torneo_id = @torneoId and dt.jugador_id = j.id order by dt.posicion";
			Dictionary<string, object> parameters = new() { { "@torneoId", torneoId } };

			joserrasQuery.ExecuteQuery( query, parameters, reader =>
			{
				while (reader.Read())
				{
					Posicion posicion = mapper.Map<SqlDataReader, Posicion>( reader );
					posiciones.Add( posicion );
				}
			} );

			return posiciones;
		}

		public List<Posicion> GetAllPosicionesByJugador(Guid jugadorId)
		{
			List<Posicion> posiciones = new List<Posicion>();

			string query = @"select dt.*, j.nombre from resultados dt, jugadores j
					where j.id = @jugadorId and dt.jugador_id = j.id";
			Dictionary<string, object> parameters = new() { { "@jugadorId", jugadorId } };

			joserrasQuery.ExecuteQuery( query, parameters, reader =>
			{
				while( reader.Read() )
				{
					Posicion posicion = mapper.Map<SqlDataReader, Posicion>( reader );
					posiciones.Add( posicion );
				}
			} );

			return posiciones;
		}

		public Torneo GetTorneo(Guid torneoId)
		{
			string query = @"select t.*, j.id as ganadorId, j.nombre as ganador
					from torneos t, resultados r, jugadores j
					where t.id = @torneoId and t.id = r.torneo_id and r.posicion = 1 and r.jugador_id = j.id";
			Dictionary<string, object> parameters = new() { { "@torneoId", torneoId } };

			Torneo torneo = joserrasQuery.ExecuteQuery( query, parameters, reader =>
			{
				reader.Read();
				return mapper.Map<SqlDataReader, Torneo>( reader );
			} );

			return torneo;
		}

		public int GetTotalTournaments()
		{
			int torneos = 0;
			string query = "select count(id) as ts from torneos";

			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while ( reader.Read() )
				{
					torneos = reader.GetFieldValue<int>( reader.GetOrdinal( "ts" ) );
				}
			} );

			return torneos;
		}

		public List<Torneo> GetAllTorneos()
		{
			List<Torneo> torneos = new List<Torneo>();
			string query = @"select t.*, j.id as ganadorId, j.nombre as ganador
					from torneos t, resultados r, jugadores j
					where t.id = r.torneo_id and r.posicion = 1 and r.jugador_id = j.id
					order by fecha desc";
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while (reader.Read())
				{
					Torneo torneo = mapper.Map<SqlDataReader, Torneo>( reader );
					torneos.Add( torneo );
				}
			} );

			return torneos;
		}

		public Torneo FindTorneoByFecha(DateTime fecha)
		{
			string query = @"select t.*, j.id as ganadorId, j.nombre as ganador
					from torneos t, resultados r, jugadores j
					where t.fecha = @fecha and t.id = r.torneo_id and r.posicion = 1 and r.jugador_id = j.id";
			Dictionary<string, object> parameters = new() { { "@fecha", fecha.ToString( "yyyy-MM-dd" ) } };

			Torneo torneo = joserrasQuery.ExecuteQuery( query, parameters, reader =>
			{
				reader.Read();
				return mapper.Map<Torneo>( reader );
			} );

			return torneo;
		}

		public List<Jugador> GetAllJugadores()
		{
			List<Jugador> jugadores = new List<Jugador>();
			string query = "select * from jugadores";
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Jugador jugador = new Jugador
					{
						Id = reader.GetGuid( 0 ),
						Nombre = reader.GetString( 1 )
					};
					jugadores.Add( jugador );
				}
			} );

			return jugadores;
		}

	}

}