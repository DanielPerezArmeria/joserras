using CsvHelper.Configuration.Attributes;

namespace TorneosWeb.domain.dto
{
	public class EliminacionesDTO
	{
		public string Jugador { get; set; }
		public string Eliminado { get; set; }

		[Default(1)]
		public int Eliminaciones { get; set; }
	}

}