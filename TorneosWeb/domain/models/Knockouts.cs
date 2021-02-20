using TorneosWeb.util;

namespace TorneosWeb.domain.models
{
	public class Knockouts
	{
		public string Nombre { get; set; }
		public string Eliminado { get; set; }

		public decimal EliminacionesNumber { get; set; }
		public string Eliminaciones
		{
			get { return EliminacionesNumber.ToString( Constants.POINTS_FORMAT ); }
		}

	}

}