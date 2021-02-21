using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.service.impl
{
	public class LigaReader : ILigaReader
	{
		private readonly IConfiguration conf;
		private IMapper mapper;
		private JoserrasQuery joserrasQuery;
		private readonly ILogger<LigaReader> Log;
		private readonly string ConnString;
		private IReadService readService;
		private IStatsService statsService;

		public LigaReader(IConfiguration conf, IReadService readService, IMapper mapper, JoserrasQuery joserrasQuery,
			IStatsService statsService, ILogger<LigaReader> log)
		{
			this.conf = conf;
			this.mapper = mapper;
			this.joserrasQuery = joserrasQuery;
			this.readService = readService;
			this.statsService = statsService;
			Log = log;
			ConnString = conf.GetConnectionString( Properties.Resources.joserrasDb );
		}

		public Liga FindLigaByNombre(string nombre)
		{
			string query = string.Format("select * from ligas where nombre = '{0}'", nombre);
			Liga liga = null;
			using( SqlConnection conn = new SqlConnection( ConnString ) )
			{
				conn.Open();

				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						liga = mapper.Map<SqlDataReader, Liga>( reader );
					}
				} );

				query = string.Format( "select torneo_id from torneos_liga where liga_id = '{0}'", liga.Id );
				List<Guid> torneosIds = new List<Guid>();
				joserrasQuery.ExecuteQuery( conn, query, reader =>
				{
					while( reader.Read() )
					{
						torneosIds.Add( (Guid)reader[ "torneo_id" ] );
					}
				} );

				liga.Torneos = readService.GetAllTorneos().Where( t => torneosIds.Contains( t.Id ) ).ToList();
			}

			liga.Estadisticas = statsService.GetStats( liga );
			liga.Standings = GetStandings( liga );

			return liga;
		}

		public List<Liga> GetAllLigas()
		{
			string query = "select * from ligas where abierta = 0 order by fecha_inicio DESC";
			List<string> nombres = new List<string>();
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					nombres.Add( reader.GetFieldValue<string>( reader.GetOrdinal( "nombre" ) ) );
				}
			} );

			List<Liga> ligas = new List<Liga>();
			foreach(string nombre in nombres )
			{
				ligas.Add( FindLigaByNombre( nombre ) );
			}

			return ligas;
		}

		public Liga GetCurrentLiga()
		{
			string query = "select * from ligas where abierta = 1";
			string nombre = "";

			joserrasQuery.ExecuteQuery( query, reader =>
					{
						while( reader.Read() )
						{
							nombre = reader.GetFieldValue<string>( reader.GetOrdinal( "nombre" ) );
						}
					} );

			if( string.IsNullOrEmpty( nombre ) )
			{
				return null;
			}

			return FindLigaByNombre(nombre);
		}

		public bool HayLigaAbierta()
		{
			throw new NotImplementedException();
		}

		public List<Standing> GetStandings(Liga liga)
		{
			if(liga == null || liga.Torneos.Count < 1 )
			{
				return new List<Standing>();
			}

			Dictionary<string, Standing> standings = new Dictionary<string, Standing>();
			foreach(Torneo torneo in liga.Torneos )
			{
				Resultados results = readService.FindResultadosTorneo( torneo.Id );
				foreach(Posicion pos in results.Posiciones )
				{
					Standing standing = null;
					if( standings.ContainsKey( pos.Nombre ) )
					{
						standing = standings[ pos.Nombre ];
					}
					else
					{
						standing = new Standing
						{
							Liga = liga,
							Jugador = pos.Nombre,
							JugadorId = pos.JugadorId
						};
						standings.Add( pos.Nombre, standing );
					}
					
					foreach(KeyValuePair<string,PointRule> rule in liga.PointRules )
					{
						standing.Puntos.TryGetValue(rule.Value.Type, out FDecimal p );
						if(p == null )
						{
							p = new FDecimal();
							standing.Puntos[ rule.Value.Type ] = p;
						}
						p.Valor += rule.Value.GetPuntaje( pos.JugadorId, liga, results );
					}
				}
			}

			if( !liga.Abierta )
			{
				string query = string.Format( "select * from puntos_torneo_liga where liga_id = '{0}'", liga.Id );
				joserrasQuery.ExecuteQuery( query, reader =>
				{
					while( reader.Read() )
					{
						Guid jugadorId = (Guid)reader[ "jugador_id" ];
						decimal premio = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio" ) );
						standings.Where( s => s.Value.JugadorId == jugadorId ).First().Value.PremioNumber = premio;
					}
				} );
			}

			List<Standing> list = standings.Values.OrderByDescending( s => s.Total ).ToList();
			foreach(Standing s in list )
			{
				decimal profit = 0M;
				foreach( Torneo torneo in liga.Torneos )
				{
					Resultados results = readService.FindResultadosTorneo( torneo.Id );
					foreach( Posicion pos in results.Posiciones.Where( p => p.Nombre == s.Jugador ) )
					{
						profit = profit + pos.ProfitNumber;
					}
				}
				s.ProfitNumber = profit;
			}

			return list;
		}

		public List<Standing> GetStandings(Liga liga, Torneo torneo)
		{
			List<Standing> standings = new List<Standing>();
			Resultados results = readService.FindResultadosTorneo( torneo.Id );
			foreach( Posicion pos in results.Posiciones )
			{
				Standing standing = new Standing
				{
					Liga = liga,
					Jugador = pos.Nombre,
					JugadorId = pos.JugadorId
				};

				foreach( KeyValuePair<string, PointRule> rule in liga.PointRules )
				{
					standing.Puntos.TryGetValue( rule.Value.Type, out FDecimal p );
					standing.Puntos[ rule.Value.Type ] = rule.Value.GetPuntaje( pos.JugadorId, liga, results ) + p;
				}

				standing.ProfitNumber = pos.ProfitNumber;

				standings.Add( standing );
			}

			return standings.OrderByDescending( s => s.Total ).ToList();
		}

	}

}