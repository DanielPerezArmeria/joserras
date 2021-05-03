namespace TorneosWeb.domain.azure
{
	public class PuntosTorneo : AbstractPuntosAzureEntity
	{
		public PuntosTorneo() : base() { }

		public PuntosTorneo(string partitionKey, string rowKey) : base( partitionKey, rowKey )
		{

		}

	}

}