using Microsoft.Azure.Cosmos.Table;

namespace TorneosWeb.util.azure
{
	public interface IAzureTableFinder
	{
		CloudTable GetTable(string tableName);
	}

}