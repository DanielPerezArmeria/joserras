using Joserras.Commons.Db;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.service.impl
{
	public class LigaReader : ILigaReader
	{
		private readonly JoserrasQuery joserrasQuery;
		private readonly ILogger<LigaReader> Log;
		private readonly IReadService readService;
		private readonly IStatsService statsService;
		private readonly ILigaDao ligaDao;

		public LigaReader(IReadService readService, JoserrasQuery joserrasQuery,
			IStatsService statsService, ILogger<LigaReader> log, ILigaDao ligaDao)
		{
			this.joserrasQuery = joserrasQuery;
			this.readService = readService;
			this.statsService = statsService;
			this.ligaDao = ligaDao;
			Log = log;
		}

		public Liga FindLigaByNombre(string nombre)
		{
			Liga liga = null;
			liga = ligaDao.FindLigaByNombre( nombre );

			string query = string.Format( "select torneo_id from torneos_liga where liga_id = '{0}'", liga.Id );
			List<Guid> torneosIds = new List<Guid>();
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					torneosIds.Add( (Guid)reader[ "torneo_id" ] );
				}
			} );

			liga.Torneos = readService.GetAllTorneos().Where( t => torneosIds.Contains( t.Id ) ).ToList();

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
			if(liga == null || liga.Torneos?.Count < 1 )
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
					
					foreach(KeyValuePair<string,PointRule> rule in liga.PointRules.Where( p => p.Value.RuleScope == RuleScope.TORNEO ))
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

			foreach (KeyValuePair<string, PointRule> rule in liga.PointRules.Where( p => p.Value.RuleScope == RuleScope.LIGA ))
			{
				foreach(Standing standing in standings.Values)
				{
					standing.Puntos[rule.Value.Type] = rule.Value.GetPuntaje( standing.JugadorId, liga, null );
				}
			}

			if ( !liga.Abierta )
			{
				string query = string.Format( "select * from puntos_torneo_liga where liga_id = '{0}'", liga.Id );
				joserrasQuery.ExecuteQuery( query, reader =>
				{
					while( reader.Read() )
					{
						Guid jugadorId = (Guid)reader[ "jugador_id" ];
						decimal premio = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio" ) );
						standings.Where( s => s.Value.JugadorId == jugadorId ).First().Value.PremioLigaNumber = premio;
					}
				} );
			}

			List<DetalleJugador> det = readService.GetAllDetalleJugador( liga );
			List<Standing> list = standings.Values.OrderByDescending( s => s.Total ).ToList();
			foreach(Standing s in list )
			{
				s.ProfitNumber = det.Single( d => d.Id == s.JugadorId ).ProfitNumber;
			}

			return list;
		}

		public List<Standing> GetStandings(Liga liga, Torneo torneo)
		{
			List<Standing> standings = new List<Standing>();
			Resultados results = torneo.Resultados;
			foreach( Posicion pos in results.Posiciones )
			{
				Standing standing = new Standing
				{
					Liga = liga,
					Jugador = pos.Nombre,
					JugadorId = pos.JugadorId
				};

				foreach( KeyValuePair<string, PointRule> rule in liga.PointRules.Where( p => p.Value.RuleScope == RuleScope.TORNEO ) )
				{
					standing.Puntos.TryGetValue( rule.Value.Type, out FDecimal p );
					standing.Puntos[ rule.Value.Type ] = rule.Value.GetPuntaje( pos.JugadorId, liga, results ) + p;
				}

				standing.ProfitNumber = pos.ProfitTorneoNumber;

				standings.Add( standing );
			}

			return standings.OrderByDescending( s => s.Total ).ToList();
		}

		public Liga GetLigaByTorneoId(Guid torneoId)
		{
			Liga liga = ligaDao.GetLigaByTorneoId( torneoId );
			liga.Estadisticas = statsService.GetStats( liga );
			liga.Standings = GetStandings( liga );
			return liga;
		}

	}

}