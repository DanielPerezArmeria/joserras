using Humanizer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TorneosWeb.domain.charts;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class ChartService : IChartService
	{
		private JoserrasQuery joserrasQuery;

		public ChartService(JoserrasQuery joserrasQuery)
		{
			this.joserrasQuery = joserrasQuery;
		}

		public ProfitChartItem GetProfitHistoryByPlayerId(Guid playerId)
		{
			List<ChartDatePoint> tourneyProfitHistory = joserrasQuery.ExecuteQuery( Properties.ChartQueries.PROFIT_HISTORY, BuildTourneyProfit,
				new SqlParameter( "playerId", playerId ) );

			List<ChartDatePoint> ligaProfitHistory = joserrasQuery.ExecuteQuery( Properties.ChartQueries.LIGA_PROFIT_HISTORY, BuildLigaProfit,
				new SqlParameter( "playerId", playerId ) );

			ProfitChartItem profitHistory = new ProfitChartItem();

			List<ChartDatePoint> consolidatedDates = new List<ChartDatePoint>( tourneyProfitHistory );
			foreach(ChartDatePoint point in tourneyProfitHistory )
			{
				if( !ligaProfitHistory.Contains( point ) )
				{
					ligaProfitHistory.Add( new ChartDatePoint( point.Year, point.Month, 0 ) );
				}
			}

			ligaProfitHistory.Sort();
			AcumularTotales( ligaProfitHistory );

			profitHistory.Tournaments = tourneyProfitHistory.Select( t => t.Total ).ToArray();
			profitHistory.Liga = ligaProfitHistory.Select( t => t.Total ).ToArray();
			profitHistory.Labels = ( from t in tourneyProfitHistory
															select new DateTime( t.Year, t.Month, 1 ).ToString( "MMM/yyyy" ).Titleize()
														).ToArray();

			return profitHistory;
		}

		private List<ChartDatePoint> BuildLigaProfit(SqlDataReader reader)
		{
			List<DateDecimalPoint> tuples = new List<DateDecimalPoint>();

			while( reader.Read() )
			{
				decimal total = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio" ) );
				tuples.Add( new DateDecimalPoint() { Date = reader.GetFieldValue<DateTime>( reader.GetOrdinal( "fecha_cierre" ) ), Total = total } );
			}

			List<ChartDatePoint> dividedDatePoints = AgruparPorMeses( tuples );

			return dividedDatePoints;
		}

		private List<ChartDatePoint> BuildTourneyProfit(SqlDataReader reader)
		{
			List<DateDecimalPoint> tuples = new List<DateDecimalPoint>();

			while( reader.Read() )
			{
				decimal total = reader.GetFieldValue<decimal>( reader.GetOrdinal( "premio" ) )
						- reader.GetFieldValue<int>( reader.GetOrdinal( "costo_total" ) );

				tuples.Add( new DateDecimalPoint() { Date = reader.GetFieldValue<DateTime>( reader.GetOrdinal( "fecha" ) ), Total = total } );
			}

			// Agrupar por mes
			List<ChartDatePoint> dividedDatePoints = AgruparPorMeses( tuples );
			AcumularTotales( dividedDatePoints );

			return dividedDatePoints;
		}

		private List<ChartDatePoint> AgruparPorMeses(List<DateDecimalPoint> tuples)
		{
			List<ChartDatePoint> dividedDatePoints = tuples.Select( t => new ChartDatePoint( t.Date.Year, t.Date.Month, t.Total ) )
				.GroupBy( x => new { x.Year, x.Month }, (key, group) => new ChartDatePoint
				(
					key.Year,
					key.Month,
					group.Sum( k => k.Total )
				) ).ToList();

			return dividedDatePoints;
		}

		private void AcumularTotales(List<ChartDatePoint> points)
		{
			decimal profitSum = 0;
			for( int i = 0; i < points.Count; i++ )
			{
				;
				points[ i ].Total = points[ i ].Total + profitSum;
				profitSum = points[ i ].Total;
			}
		}

	}

}