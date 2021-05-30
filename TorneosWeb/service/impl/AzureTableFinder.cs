using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TorneosWeb.config;
using TorneosWeb.domain.azure;

namespace TorneosWeb.service.impl
{
	public class AzureTableFinder : IAzureTableFinder
	{
		private CloudStorageAccount storageAccount;
		private readonly AzureTableConfig azureTableConfig;
		private readonly ILogger<AzureTableFinder> log;
		private readonly string connString;
		private readonly IDictionary<string, CloudTable> tables;

		public AzureTableFinder(IOptions<AzureTableConfig> azureTableConfig, ILogger<AzureTableFinder> logger)
		{
			this.azureTableConfig = azureTableConfig.Value;
			connString = this.azureTableConfig.Storage;
			tables = new Dictionary<string, CloudTable>();
			log = logger;
			Init();
		}

		public void Init()
		{
			try
			{
				storageAccount = CloudStorageAccount.Parse( connString );

				CloudTableClient tableClient = storageAccount.CreateCloudTableClient( new TableClientConfiguration() );

				//CloudTable tableLiga = tableClient.GetTableReference( nameof(PuntosLiga) );
				//CloudTable tableTorneo = tableClient.GetTableReference( nameof( PuntosTorneo ) );

				//tableLiga.CreateIfNotExists();
				//tableTorneo.CreateIfNotExists();
			}
			catch (FormatException e)
			{
				log.LogError( e, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application." );
				if (azureTableConfig.Enabled)
				{
					throw;
				}
			}
			catch (ArgumentException e)
			{
				log.LogError( e, "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application." );
				if (azureTableConfig.Enabled)
				{
					throw;
				}
			}
		}

		public CloudTable GetTable(string tableName)
		{
			if (!tables.ContainsKey( tableName ))
			{
				CloudTableClient tableClient = storageAccount.CreateCloudTableClient( new TableClientConfiguration() );
				tables.Add( tableName, tableClient.GetTableReference( tableName ) );
			}

			return tables[tableName];
		}

	}

}