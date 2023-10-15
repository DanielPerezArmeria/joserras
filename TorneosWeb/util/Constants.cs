namespace TorneosWeb.util
{
	public static class Constants
	{
		public const string CURRENCY_FORMAT = "$###,##0.0#";
		public const string BOLSA_FORMAT = "$###,###";
		public const string PRIZE_CURRENCY_FORMAT = "$###,##0.##";
		public const string ROI_FORMAT = "%###,##0.0";
		public const string POINTS_FORMAT = "###,##0.##";
		public const string FECHA_LIGA_FORMAT = "MMMM yyyy";
		public const string LISTADO_TORNEOS = "Torneos";
		public const string LISTADO_LIGA = "Liga";
	}

	public static class PrizeFill
	{
		public const string PERCENT_CHAR = "%";
		public const string PERCENT_AND_REMOVE = "&";
		public const string FACTOR = "x";
		public const string SEPARATOR = "-";
	}

	public static class AzureTables
	{
		public const string PUNTOS_TORNEO_TABLE = "puntostorneo";
		public const string PUNTOS_LIGA_TABLE = "puntosliga";
	}

}