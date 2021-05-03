using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.azure;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;
using TorneosWeb.util.azure;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.dao.impl
{
	public class AzureTableStorageDao : IStorageDao
	{
		private readonly ILogger<AzureTableStorageDao> log;
		private readonly IAzureTableFinder tableFinder;

		public AzureTableStorageDao(IAzureTableFinder tableFinder, ILogger<AzureTableStorageDao> logger)
		{
			this.tableFinder = tableFinder;
      log = logger;
		}

		public void SaveTorneoStandings<T>(string tableName, Guid torneoId, List<Standing> standings) where T : AbstractPuntosAzureEntity
		{
			CloudTable table = tableFinder.GetTable( tableName );

			TableBatchOperation batchOperation = new();

			foreach (Standing standing in standings)
			{
				T puntos = (T)Activator.CreateInstance( typeof(T), new object[] { torneoId.ToString(), standing.JugadorId.ToString() } );
				puntos.Standings = new SortedDictionary<PointRuleType, FDecimal>( standing.Puntos );

				batchOperation.InsertOrReplace( puntos );
			}

			log.LogDebug( "Saving points for torneoId: '{0}'", torneoId );
			table.ExecuteBatch( batchOperation );
			log.LogDebug( "Records inserted" );
		}

	}

}