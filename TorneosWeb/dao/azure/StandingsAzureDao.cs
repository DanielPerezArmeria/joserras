using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.azure;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.dao.azure
{
	public class StandingsAzureDao<T> : IStandingsDao<T> where T : AbstractPuntosAzureEntity
	{
		private IAzureTableFinder tableFinder;
		private ILogger<StandingsAzureDao<T>> log;

		public StandingsAzureDao(IAzureTableFinder tableFinder, ILogger<StandingsAzureDao<T>> logger)
		{
			this.tableFinder = tableFinder;
			log = logger;
		}

		public void Save(Guid id, List<Standing> standings)
		{
			CloudTable table = tableFinder.GetTable( nameof(T) );

			TableBatchOperation batchOperation = new();

			foreach (Standing standing in standings)
			{
				AbstractPuntosAzureEntity puntos = (T)Activator.CreateInstance( typeof( T ), new object[] { id.ToString(), standing.JugadorId.ToString() } );
				puntos.Standings = new SortedDictionary<PointRuleType, FDecimal>( standing.Puntos );

				batchOperation.InsertOrReplace( puntos );
			}

			log.LogDebug( "Saving points in table '{0}' for Id '{1}'", nameof( T ), id );
			table.ExecuteBatch( batchOperation );
			log.LogDebug( "Records inserted" );
		}

	}

}