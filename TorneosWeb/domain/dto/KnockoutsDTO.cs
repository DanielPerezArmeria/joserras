using CsvHelper.Configuration.Attributes;

namespace TorneosWeb.domain.dto
{
	public class KnockoutsDTO
	{
		public KnockoutsDTO() { }

		public KnockoutsDTO(string jugador, string eliminado, decimal kos)
		{
			Jugador = jugador;
			Eliminado = eliminado;
			Eliminaciones = kos;
		}

		public string Jugador { get; set; }
		public string Eliminado { get; set; }

		[Default(1.0)]
		public decimal Eliminaciones { get; set; }
	}

}