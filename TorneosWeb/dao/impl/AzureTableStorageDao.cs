using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TorneosWeb.config;
using TorneosWeb.dao.azure;
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

		public AzureTableStorageDao(IOptions<AzureTableConfig> azureTableConfig, IAzureTableFinder tableFinder, ILogger<AzureTableStorageDao> logger)
		{
			this.tableFinder = tableFinder;
      log = logger;
		}

		public void SaveTorneoStandings(string tableName, Guid torneoId, List<Standing> standings)
		{
			CloudTable table = tableFinder.GetTable( tableName );

			TableBatchOperation batchOperation = new();

			foreach (Standing standing in standings)
			{
				PuntosTorneo puntos = new( torneoId.ToString(), standing.JugadorId.ToString() );
				puntos.Standings = new SortedDictionary<PointRuleType, FDecimal>( standing.Puntos );

				batchOperation.InsertOrReplace( puntos );
			}

			log.LogDebug( "Saving points for torneoId: '{0}'", torneoId );
			table.ExecuteBatch( batchOperation );
			log.LogDebug( "Records inserted" );
		}

	}

}