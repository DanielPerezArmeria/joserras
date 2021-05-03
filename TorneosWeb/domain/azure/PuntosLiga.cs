namespace TorneosWeb.domain.azure
{
	public class PuntosLiga : AbstractPuntosAzureEntity
	{
		public PuntosLiga() : base() { }

		public PuntosLiga(string partitionKey, string rowKey) : base( partitionKey, rowKey )
		{
			
		}

	}

}