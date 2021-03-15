﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class ReadServiceImpl : IReadService
	{
		private readonly string connString;
		private IMapper mapper;
		private JoserrasQuery joserrasQuery;
		private ILigaDao ligaDao;

		public ReadServiceImpl(IConfiguration conf, IMapper mapper, JoserrasQuery joserrasQuery, ILigaDao ligaDao)
		{
			connString = conf.GetConnectionString( Properties.Resources.joserrasDb );
			this.mapper = mapper;
			this.joserrasQuery = joserrasQuery;
			this.ligaDao = ligaDao;
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


		#region GetAllKnockouts
		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			return GetAllKnockouts( string.Empty );
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(DateTime start, DateTime end)
		{
			return GetAllKnockouts( QueryUtils.FormatTorneoBetween( start, end ) );
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(Liga liga)
		{
			return GetAllKnockouts( QueryUtils.FormatTorneoIdIn( liga ) );
		}

		private SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(string q)
		{
			SortedList<string, Dictionary<string, Knockouts>> knockouts = new SortedList<string, Dictionary<string, Knockouts>>();
			string query = string.Format( Properties.Queries.GetAllKOs, q );

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
		#endregion


		#region Get Torneos

		public List<Torneo> GetAllTorneos()
		{
			List<Torneo> torneos = new List<Torneo>();

			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();

				string query = "select * from torneos order by fecha desc";
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						Torneo torneo = mapper.Map<SqlDataReader, Torneo>( reader );
						torneo.Liga = ligaDao.GetLigaByTorneoId( torneo.Id );
						torneos.Add( torneo );
					}
				} );

				FillTorneos( torneos, conn );
			}

			return torneos;
		}

		private void FillTorneos(Torneo torneo, SqlConnection conn)
		{
			FillTorneos( new List<Torneo>() { torneo }, conn );
		}

		private void FillTorneos(List<Torneo> torneos, SqlConnection conn)
		{
			foreach( Torneo t in torneos )
			{
				// Obtener Resultados
				t.Resultados = FindResultadosTorneo( conn, t.Id );
			}
		}

		public Torneo FindTorneoByFecha(DateTime fecha)
		{
			Torneo torneo = null;
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();
				string query = string.Format( @"select * from torneos where fecha = '{0}'", fecha.ToString( "yyyy-MM-dd" ) );
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						torneo = mapper.Map<Torneo>( reader );
					}
				} );
			}

			return torneo;
		}

		#endregion


		public Resultados FindResultadosTorneo(Guid torneoId)
		{
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();
				return FindResultadosTorneo( conn, torneoId );
			}
		}

		private Resultados FindResultadosTorneo( SqlConnection conn, Guid torneoId)
		{
			Resultados resultados = new Resultados();
			
			string query = string.Format( "select * from torneos where id = '{0}'", torneoId );
			Torneo torneo = joserrasQuery.ExecuteQuery( conn, query, reader =>
			{
				reader.Read();
				return mapper.Map<SqlDataReader, Torneo>( reader );
			} );

			resultados.Torneo = torneo;

			resultados.Torneo.Liga = ligaDao.GetLigaByTorneoId( torneoId );

			query = string.Format( "select dt.*, j.nombre from resultados dt, jugadores j "
					+ "where dt.torneo_id = '{0}' and dt.jugador_id = j.id order by dt.posicion", torneoId );
			List<Posicion> posiciones = new List<Posicion>();
			resultados.Jugadores = new SortedSet<string>();
			joserrasQuery.ExecuteQuery( conn, query, reader =>
			{
				while( reader.Read() )
				{
					Posicion posicion = mapper.Map<SqlDataReader, Posicion>( reader );
					decimal premio = 0;
					try
					{
						premio = decimal.Parse( posicion.Premio, System.Globalization.NumberStyles.Currency );
					}
					catch( FormatException ) { }
					int costos = resultados.Torneo.PrecioBuyinNumber + (posicion.Rebuys * resultados.Torneo.PrecioRebuyNumber);
					posicion.ProfitNumber = premio + posicion.PremioBountiesNumber - costos;
					posiciones.Add( posicion );
					resultados.Jugadores.Add( posicion.Nombre );
				}
			} );

			resultados.Posiciones = posiciones;
			
			resultados.Knockouts = GetKnockoutsByTournamentId( conn, torneoId );

			return resultados;
		}

		public DetalleJugador FindDetalleJugador(Guid playerId)
		{
			DetalleJugador detalle = null;
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();

				string query = string.Format( Properties.Queries.GetDetalleJugador, playerId );
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					if( reader.HasRows )
					{
						reader.Read();
						detalle = mapper.Map<DetalleJugador>( reader );
					}
				} );

				// Si el Detalle es nulo, es que el jugador no ha jugado ningún torneo
				if(detalle == null )
				{
					detalle = new DetalleJugador();
				}

				//Encontrar profits de ligas
				query = string.Format("select sum(premio) as premio_liga from puntos_torneo_liga where jugador_id = '{0}'", playerId.ToString());
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						decimal premio = 0;
						try
						{
							premio = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio_liga" ) );
						}
						catch( SqlNullValueException ) { }

						detalle.PremiosLigaNumber = premio;
					}
				} );
			}

			detalle.Knockouts = GetKnockoutsByPlayer( playerId );

			return detalle;
		}

		public DetalleJugador FindDetalleJugador(string nombre)
		{
			throw new NotImplementedException();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockoutsByTournamentId(Guid torneoId)
		{
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();

				return GetKnockoutsByTournamentId( conn, torneoId );
			}
		}

		private SortedList<string, Dictionary<string, Knockouts>> GetKnockoutsByTournamentId(SqlConnection conn, Guid torneoId)
		{
			SortedList<string, Dictionary<string, Knockouts>> knockouts = new SortedList<string, Dictionary<string, Knockouts>>();
			string query = string.Format( "select j.nombre, elim.nombre as eliminado, sum(e.eliminaciones) as eliminaciones from knockouts e, jugadores j, jugadores elim"
					+ " where torneo_id = '{0}' and e.jugador_id = j.id and e.eliminado_id = elim.id group by j.nombre, elim.nombre", torneoId );

			joserrasQuery.ExecuteQuery( conn, query, reader =>
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


		#region GetAllDetalleJugador

		public List<DetalleJugador> GetAllDetalleJugador()
		{
			List<DetalleJugador> detalles = GetAllDetalleJugador( string.Empty );

			//Encontrar profits de ligas
			string query = "select jugador_id, sum(premio) as premio_liga from puntos_torneo_liga group by jugador_id";
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Guid jugadorId = (Guid)reader[ "jugador_id" ];
					decimal premio = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio_liga" ) );
					DetalleJugador detalle = detalles.Where( d => d.Id == jugadorId ).First();
					detalle.PremiosLigaNumber = premio;
				}
			} );

			

			return detalles;
		}

		public List<DetalleJugador> GetAllDetalleJugador(DateTime start, DateTime end)
		{
			List<DetalleJugador> detalles = GetAllDetalleJugador( QueryUtils.FormatTorneoBetween( start, end ) );

			return detalles;
		}

		public List<DetalleJugador> GetAllDetalleJugador(Liga liga)
		{
			List<DetalleJugador> detalles = GetAllDetalleJugador( QueryUtils.FormatTorneoIdIn( liga ) );

			//Encontrar profits de la liga
			string query = "select jugador_id, sum(premio) as premio_liga from puntos_torneo_liga where liga_id = '{0}' group by jugador_id";
			query = string.Format( query, liga.Id );
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Guid jugadorId = (Guid)reader[ "jugador_id" ];
					decimal premio = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio_liga" ) );
					DetalleJugador detalle = detalles.Where( d => d.Id == jugadorId ).First();
					detalle.PremiosLigaNumber = premio;
				}
			} );

			return detalles;
		}

		private List<DetalleJugador> GetAllDetalleJugador(string q)
		{
			List<DetalleJugador> detalles = new List<DetalleJugador>();
			using( SqlConnection conn = new SqlConnection( connString ) )
			{
				conn.Open();

				string query = string.Format( Properties.Queries.GetStats, q );
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

		#endregion

	}

}