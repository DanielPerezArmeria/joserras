using System.Runtime.Serialization;

namespace TorneosWeb.domain
{
	[DataContract]
	public class ChartDataPoint
	{
		public ChartDataPoint()
		{
			Label = "";
			Profit = null;
		}

		public ChartDataPoint(string label, decimal? profit)
		{
			Label = label;
			Profit = profit;
		}


		[DataMember( Name = "label" )]
		public string Label { get; set; }

		[DataMember( Name = "y" )]
		public decimal? Profit { get; set; }
	}

}