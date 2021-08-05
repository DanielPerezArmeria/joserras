using AutoMapper;
using Joserras.Commons.Db;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.dao.impl
{
	public class LigaDao : ILigaDao
	{
		private JoserrasQuery joserrasQuery;
		private IMapper mapper;
		private ILogger<LigaDao> log;
		private IPointRuleFactory pointRuleFactory;

		public LigaDao(JoserrasQuery joserrasQuery, IMapper mapper, ILogger<LigaDao> logger, IPointRuleFactory pointRuleFactory)
		{
			this.joserrasQuery = joserrasQuery;
			this.mapper = mapper;
			log = logger;
			this.pointRuleFactory = pointRuleFactory;
		}

		public Liga GetLigaByTorneoId(Guid torneoId)
		{
			string query = string.Format( "select * from ligas where id = (select liga_id from torneos_liga where torneo_id = '{0}')", torneoId );
			return ExecuteLigaQuery( query ); ;
		}

		public Liga FindLigaByNombre(string nombre)
		{
			string query = string.Format( "select * from ligas where nombre = '{0}'", nombre );
			return ExecuteLigaQuery( query );
		}

		public Liga FindCurrentLiga()
		{
			return ExecuteLigaQuery( "select * from ligas where abierta = true" );
		}

		private Liga ExecuteLigaQuery(string query)
		{
			Liga liga = null;
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while (reader.Read())
				{
					liga = mapper.Map<SqlDataReader, Liga>( reader );
					liga.PointRules = pointRuleFactory.BuildRules( liga.Puntaje );
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