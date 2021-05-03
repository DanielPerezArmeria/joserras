using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TorneosWeb.config;

namespace TorneosWeb.util.azure
{
	public class ProdAzureTableFinder : IAzureTableFinder
	{
		private CloudStorageAccount storageAccount;
		private readonly AzureTableConfig azureTableConfig;
		private readonly ILogger<ProdAzureTableFinder> log;
		private readonly string connString;
		private readonly IDictionary<string, CloudTable> tables;

		public ProdAzureTableFinder(IOptions<AzureTableConfig> azureTableConfig, ILogger<ProdAzureTableFinder> logger)
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