using Humanizer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TorneosWeb.domain.charts;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class ChartService : IChartService
	{
		private JoserrasQuery joserrasQuery;
		private IReadService readService;
		private ILigaReader ligaReader;

		public ChartService(JoserrasQuery joserrasQuery, IReadService readService, ILigaReader ligaReader)
		{
			this.joserrasQuery = joserrasQuery;
			this.readService = readService;
			this.ligaReader = ligaReader;
		}

		public ProfitChartItem GetProfitHistoryByPlayerId(Guid playerId)
		{
			List<ChartDatePoint> tourneyProfitHistory = joserrasQuery.ExecuteQuery( Properties.ChartQueries.PROFIT_HISTORY, BuildTourneyProfit,
				new SqlParameter( "playerId", playerId ) );

			ProfitChartItem chartItem = new ProfitChartItem();

			chartItem.Tournaments = tourneyProfitHistory.Select( t => t.Total ).ToArray();
			chartItem.Labels = ( from t in tourneyProfitHistory
															select new DateTime( t.Year, t.Month, 1 ).ToString( "MMM/yyyy" ).Titleize()
														).ToArray();

			GetLigaProfits( chartItem, playerId );

			return chartItem;
		}

		private void GetLigaProfits(ProfitChartItem chartItem, Guid playerId)
		{
			List<string> ligaLabels = new List<string>();
			List<decimal> ligaProfits = new List<decimal>();

			List<Liga> ligas = new List<Liga>( ligaReader.GetAllLigas() );
			ligas.Reverse();
			Liga current = ligaReader.GetCurrentLiga();
			if(current != null ) { ligas.Add( current ); }

			decimal sum = 0;
			foreach(Liga liga in ligas )
			{
				List<DetalleJugador> detalles = readService.GetAllDetalleJugador( liga );
				DetalleJugador detalle = detalles.SingleOrDefault( d => d.Id == playerId );
				if(detalle != null )
				{
					decimal item = sum + detalle.ProfitLigasNumber;
					ligaProfits.Add( item );
					sum = item;
					ligaLabels.Add( liga.Nombre );
				}
			}

			chartItem.Liga = ligaProfits.ToArray();
			chartItem.Ligalabels = ligaLabels.ToArray();
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
				points[ i ].Total = points[ i ].Total + profitSum;
				profitSum = points[ i ].Total;
			}
		}

	}

}