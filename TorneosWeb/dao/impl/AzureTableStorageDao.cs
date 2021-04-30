using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TorneosWeb.config;
using TorneosWeb.dao.azure;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.dao.impl
{
	public class AzureTableStorageDao : IStorageDao
	{
		private readonly string connString;
    private readonly AzureTableConfig azureTableConfig;
    private readonly ILogger<AzureTableStorageDao> log;
    private readonly IDictionary<string,CloudTable> tables;
		CloudStorageAccount storageAccount;

		public AzureTableStorageDao(IOptions<AzureTableConfig> azureTableConfig, ILogger<AzureTableStorageDao> logger)
		{
			this.azureTableConfig = azureTableConfig.Value;
      connString = this.azureTableConfig.Storage;
      log = logger;
      tables = new Dictionary<string,CloudTable>();
      Init();
		}

		public void Init()
		{
			try
			{
				storageAccount = CloudStorageAccount.Parse( connString );
			}
			catch (FormatException)
			{
				Console.WriteLine( "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application." );
				throw;
			}
			catch (ArgumentException)
			{
				Console.WriteLine( "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample." );
				Console.ReadLine();
				throw;
			}
		}

		private CloudTable GetTable(string tableName)
		{
			if (!tables.ContainsKey( tableName ))
			{
				CloudTableClient tableClient = storageAccount.CreateCloudTableClient( new TableClientConfiguration() );
				CloudTable table = tableClient.GetTableReference( tableName );
				table.DeleteIfExists();
				table.CreateIfNotExists();

				tables.Add( tableName, tableClient.GetTableReference( tableName ) );
			}

			return tables[tableName];
		}

		public void SaveTorneoStandings(string tableName, Guid torneoId, List<Standing> standings)
		{
			CloudTable table = GetTable( tableName );

			TableBatchOperation batchOperation = new TableBatchOperation();

			foreach (Standing standing in standings)
			{
				PuntosTorneo puntos = new( torneoId.ToString(), standing.JugadorId.ToString() );
				puntos.Standings = new SortedDictionary<PointRuleType, FDecimal>( standing.Puntos );

				batchOperation.Insert( puntos );
			}

			log.LogDebug( "Saving points for torneoId: '{0}'", torneoId );
			table.ExecuteBatch( batchOperation );
			log.LogDebug( "Records inserted" );
		}

	}

}