using CsvHelper.Configuration.Attributes;

namespace TorneosWeb.domain.dto
{
	public class KnockoutsDTO
	{
		public KnockoutsDTO() { }

		public KnockoutsDTO(string jugador, string eliminado)
		{
			Jugador = jugador;
			Eliminado = eliminado;
		}

		public KnockoutsDTO(string jugador, string eliminado, string manoUrl)
		{
			Jugador = jugador;
			Eliminado = eliminado;
			Mano = manoUrl;
		}

		public string Jugador { get; set; }
		public string Eliminado { get; set; }

		[Default(null)]
		public string Mano { get; set; }
	}

}