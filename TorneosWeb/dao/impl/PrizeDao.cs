using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.util;

namespace TorneosWeb.dao.impl
{
	public class PrizeDao : IPrizeDao
	{
		private readonly string ConnString;
		private JoserrasQuery joserrasQuery;

		public PrizeDao(IConfiguration conf, JoserrasQuery joserrasQuery)
		{
			ConnString = conf.GetConnectionString( Properties.Resources.joserrasDb );
			this.joserrasQuery = joserrasQuery;
		}

		public IEnumerable<PrizeRange> GetPrizeRanges()
		{
			List<PrizeRange> prizeRanges = new List<PrizeRange>();

			string query = "select * from premiaciones";
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					string range = reader.GetString( 1 );
					string premio = reader.GetString( 2 );
					string[] ranges = range.Split( "-" );
					if( ranges.Length > 1 )
					{
						prizeRanges.Add( new PrizeRange( int.Parse( ranges[ 0 ] ), int.Parse( ranges[ 1 ] ), premio ) );
					}
					else
					{
						prizeRanges.Add( new PrizeRange( int.Parse( ranges[ 0 ] ), int.MaxValue, premio ) );
					}
				}
			} );

			return prizeRanges;
		}

	}

}