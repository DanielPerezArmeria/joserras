using Microsoft.Azure.Cosmos.Table;

namespace TorneosWeb.service
{
	public interface IAzureTableFinder
	{
		CloudTable GetTable(string tableName);
	}

}