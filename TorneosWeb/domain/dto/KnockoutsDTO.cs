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

		public KnockoutsDTO(string jugador, string eliminado, decimal kos, string manoUrl)
		{
			Jugador = jugador;
			Eliminado = eliminado;
			Eliminaciones = kos;
			Mano = manoUrl;
		}

		public string Jugador { get; set; }
		public string Eliminado { get; set; }

		[Default(1.0)]
		public decimal Eliminaciones { get; set; }

		public string Mano { get; set; }
	}

}