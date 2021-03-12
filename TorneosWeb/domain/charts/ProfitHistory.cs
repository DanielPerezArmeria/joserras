namespace TorneosWeb.domain.charts
{
	public class ProfitHistory
	{
		public ProfitEntry[] TournamentProfitHistory { get; set; }
		public ProfitEntry[] LigaProfitHistory { get; set; }

		public class ProfitEntry
		{
			public string Fecha { get; set; }
			public decimal Profit { get; set; }

			public ProfitEntry() { }

			public ProfitEntry(string fecha, decimal profit)
			{
				Fecha = fecha;
				Profit = profit;
			}
		}

	}

}