using Joserras.Commons.Db;
using Joserras.Commons.Domain;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace TorneosWeb.dao.impl
{
	public class PrizeDao : IPrizeDao
	{
		private JoserrasQuery joserrasQuery;

		public PrizeDao(IConfiguration conf, JoserrasQuery joserrasQuery)
		{
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