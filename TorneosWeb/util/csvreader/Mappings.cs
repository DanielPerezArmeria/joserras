using CsvHelper.Configuration;
using Joserras.Commons.Dto;
using System.Globalization;

namespace TorneosWeb.util.csvreader
{
	public class KnockoutsMap : ClassMap<KnockoutsDTO>
	{
		public KnockoutsMap()
		{
			AutoMap( CultureInfo.InvariantCulture );
			Map( m => m.Eliminaciones ).Default( 1 );
		}
	}

	public class ResultadosMap : ClassMap<ResultadosDTO>
	{
		public ResultadosMap()
		{
			AutoMap( CultureInfo.InvariantCulture );
			Map( m => m.Puntualidad ).Default( true );
			Map( m => m.Rebuys ).Default( 0 );
			Map( m => m.Premio ).Default( "0" );
		}
	}

}