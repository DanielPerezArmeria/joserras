using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TorneosWeb.domain.charts;
using TorneosWeb.util;
using static TorneosWeb.domain.charts.ProfitHistory;

namespace TorneosWeb.service.impl
{
	public class ChartService : IChartService
	{
		private JoserrasQuery joserrasQuery;

		public ChartService(JoserrasQuery joserrasQuery)
		{
			this.joserrasQuery = joserrasQuery;
		}

		public ProfitHistory GetProfitHistoryByPlayerId(Guid playerId)
		{
			ProfitEntry[] tourneyProfitHistory = joserrasQuery.ExecuteQuery( Properties.ChartQueries.PROFIT_HISTORY, BuildTourneyProfit,
				new SqlParameter( "playerId", playerId ) );
			
			ProfitEntry[] ligaProfitHistory = joserrasQuery.ExecuteQuery( Properties.ChartQueries.LIGA_PROFIT_HISTORY, BuildLigaProfit,
				new SqlParameter( "playerId", playerId ) );

			ProfitHistory profitHistory = new ProfitHistory();
			profitHistory.TournamentProfitHistory = tourneyProfitHistory;
			profitHistory.LigaProfitHistory = ligaProfitHistory;

			return profitHistory;
		}

		private ProfitEntry[] BuildLigaProfit(SqlDataReader reader)
		{
			List<ProfitEntry> history = new List<ProfitEntry>();
			while( reader.Read() )
			{
				ProfitEntry hist = new ProfitEntry();
				hist.Fecha = reader.GetFieldValue<DateTime>( reader.GetOrdinal( "fecha_cierre" ) ).ToShortDateString();
				hist.Profit = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio" ) );
				history.Add( hist );
			}
			return history.ToArray();
		}

		private ProfitEntry[] BuildTourneyProfit(SqlDataReader reader)
		{
			List<ProfitEntry> history = new List<ProfitEntry>();
			while( reader.Read() )
			{
				ProfitEntry hist = new ProfitEntry();
				hist.Fecha = reader.GetFieldValue<DateTime>( reader.GetOrdinal( "fecha" ) ).ToShortDateString();
				hist.Profit = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio" ) ) -
						reader.GetFieldValue<int>( reader.GetOrdinal( "costo_total" ) );
				history.Add( hist );
			}
			return history.ToArray();
		}

	}

}