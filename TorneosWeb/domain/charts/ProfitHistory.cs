using System;

namespace TorneosWeb.domain.charts
{
	public class ProfitChartItem
	{
		public string[] Labels { get; set; }
		public decimal[] Tournaments { get; set; }
		public decimal[] Liga { get; set; }

	}

	public class DateDecimalPoint
	{
		public DateTime Date { get; set; }
		public decimal Total { get; set; }
	}

	public class ChartDatePoint : IComparable<ChartDatePoint>, IEquatable<ChartDatePoint>
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public decimal Total { get; set; }

		public ChartDatePoint(int year, int month, decimal total)
		{
			Year = year;
			Month = month;
			Total = total;
		}

		public int CompareTo(ChartDatePoint other)
		{
			if( Year == other.Year )
			{
				if(Month == other.Month ) { return 0; }
				else if(Month < other.Month ) { return -1; }
				else { return 1; }
			}
			else if(Year < other.Year ) { return -1; }
			else { return 1; }
		}

		public bool Equals(ChartDatePoint other)
		{
			return Year == other.Year && Month == other.Month;
		}

	}

}