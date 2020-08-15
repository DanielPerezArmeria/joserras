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
		private IConfiguration conf;
		private IMapper mapper;
		private JoserrasQuery joserrasQuery;
		private ILogger<LigaReader> Log;
		private string ConnString;
		private IReadService readService;

		public LigaReader(IConfiguration conf, IReadService readService, IMapper mapper, JoserrasQuery joserrasQuery, ILogger<LigaReader> log)
		{
			this.conf = conf;
			this.mapper = mapper;
			this.joserrasQuery = joserrasQuery;
			this.readService = readService;
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
						standing = new Standing();
						standing.Jugador = pos.Nombre;
						standings.Add( pos.Nombre, standing );
					}
					
					foreach(KeyValuePair<string,PointRule> rule in liga.PointRules )
					{
						standing.Puntos.TryGetValue(rule.Value.Type, out int p );
						standing.Puntos[ rule.Value.Type ] = rule.Value.GetPuntaje( pos.JugadorId, liga, results ) + p;
					}
				}
			}

			List<Standing> list = standings.Values.OrderByDescending( s => s.Total ).ToList();
			foreach(Standing s in list )
			{
				int profit = 0;
				foreach( Torneo torneo in liga.Torneos )
				{
					Resultados results = readService.FindResultadosTorneo( torneo.Id );
					foreach( Posicion pos in results.Posiciones.Where( p => p.Nombre == s.Jugador ) )
					{
						profit = profit + pos.ProfitNumber;
					}
				}
				s.Profit = profit.ToString( Constants.CURRENCY_FORMAT );
				s.ProfitNumber = profit;
			}

			return list;
		}

	}

}