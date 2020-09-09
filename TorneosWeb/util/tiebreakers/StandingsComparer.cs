using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.tiebreakers
{
	public class StandingsComparer : AbstractStandingComparer
	{
		private List<AbstractStandingComparer> comparers;

		public StandingsComparer(List<AbstractStandingComparer> comparers)
		{
			this.comparers = comparers;
		}

		public override string Description
		{
			get
			{
				string d = string.Empty;
				try
				{
					int i = 1;
					d = string.Join( "; ", comparers.Select( c => comparers.IndexOf(c).Ordinalize() + " " + c.Description ).ToArray(), 1, comparers.Count - 1 );
				}
				catch( Exception ) { }

				return d;
			}
		}

		public override TiebreakerType Type => TiebreakerType.BASE;

		public override int Compare(Standing x, Standing y)
		{
			for( int i = 0; i < comparers.Count; i++ )
			{
				Comparer<Standing> com = comparers.ElementAt( i );
				int c = com.Compare( x, y );
				if( c == 0 )
				{
					continue;
				}
				else
				{
					return c;
				}
			}
			return 0;
		}

		public static StandingsComparer Build(string desempate)
		{
			List<AbstractStandingComparer> comparers = new List<AbstractStandingComparer>();
			comparers.Add( new PuntosComparer() );

			if( string.IsNullOrEmpty( desempate ) )
			{
				return new StandingsComparer( comparers );
			}

			string[] desempatesArray = desempate.Split( ";" );
			foreach( string tiebreaker in desempatesArray )
			{
				TiebreakerType type = (TiebreakerType)Enum.Parse( typeof( TiebreakerType ), tiebreaker );

				switch( type )
				{
					case TiebreakerType.KOS_DIRECTOS:
						comparers.Add( new KosDirectosComparer() );
						break;

					case TiebreakerType.PROFIT:
						comparers.Add( new ProfitsComparer() );
						break;

					default:
						break;
				}
			}

			return new StandingsComparer( comparers );
		}

	}

}