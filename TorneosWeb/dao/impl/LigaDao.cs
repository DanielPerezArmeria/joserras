using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;

namespace TorneosWeb.dao.impl
{
	public class LigaDao : ILigaDao
	{
		private JoserrasQuery joserrasQuery;
		private IMapper mapper;
		private ILogger<LigaDao> log;

		public LigaDao(JoserrasQuery joserrasQuery, IMapper mapper, ILogger<LigaDao> logger)
		{
			this.joserrasQuery = joserrasQuery;
			this.mapper = mapper;
			log = logger;
		}

		public Liga GetLigaByTorneoId(Guid torneoId)
		{
			string query = string.Format( "select * from ligas where id = (select liga_id from torneos_liga where torneo_id = '{0}')", torneoId );
			Liga liga = null;
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					liga = mapper.Map<SqlDataReader, Liga>( reader );
				}
			} );

			return liga;
		}

		public LigaProfitsObject GetTotalLigaProfitsByPlayerId(Guid playerId)
		{
			string query = string.Format( Properties.Queries.GetTotalLigaProfitsByPlayerId, playerId.ToString() );
			LigaProfitsObject profits = new LigaProfitsObject();
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					try
					{
						profits = mapper.Map<SqlDataReader, LigaProfitsObject>( reader );
					}
					catch( Exception e )
					{
						log.LogError( e, e.Message );
						throw;
					}
				}
			} );

			return profits;
		}

		public IEnumerable<LigaProfitsObject> GetTotalLigaProfits()
		{
			List<LigaProfitsObject> profitObjects = new List<LigaProfitsObject>();
			string query = string.Format( Properties.Queries.GetTotalLigaProfits );
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					try
					{
						profitObjects.Add( mapper.Map<SqlDataReader, LigaProfitsObject>( reader ) );
					}
					catch( Exception e )
					{
						log.LogError( e, e.Message );
						throw;
					}
				}
			} );

			return profitObjects;
		}

		public IEnumerable<LigaProfitsObject> GetLigaProfitsByLiga(Liga liga)
		{
			List<LigaProfitsObject> profitObjects = new List<LigaProfitsObject>();
			string query = string.Format( Properties.Queries.GetLigaProfitsByLiga, liga.Id );
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					try
					{
						profitObjects.Add( mapper.Map<SqlDataReader, LigaProfitsObject>( reader ) );
					}
					catch( Exception e )
					{
						log.LogError( e, e.Message );
						throw;
					}
				}
			} );

			return profitObjects;
		}

	}

}