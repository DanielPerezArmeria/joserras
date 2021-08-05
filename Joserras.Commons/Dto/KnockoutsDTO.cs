namespace Joserras.Commons.Dto
{
	public class KnockoutsDTO
	{
		public KnockoutsDTO() { }

		public KnockoutsDTO(string jugador, string eliminado)
		{
			Jugador = jugador;
			Eliminado = eliminado;
			Eliminaciones = 1;
		}

		public KnockoutsDTO(string jugador, string eliminado, string manoUrl)
		{
			Jugador = jugador;
			Eliminado = eliminado;
			Mano = manoUrl;
			Eliminaciones = 1;
		}

		public string Jugador { get; set; }
		public string Eliminado { get; set; }
		public decimal Eliminaciones { get; set; }
		public string Mano { get; set; }
	}

}