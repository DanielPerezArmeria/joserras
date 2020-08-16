using System;
using System.Linq;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util
{
	public static class QueryUtils
	{
		public static string FormatTorneoBetween(DateTime start, DateTime end)
		{
			string q = string.Format( "and t.fecha between '{0}' and '{1}'", start.ToString( "yyyy-MM-dd" ), end.ToString( "yyyy-MM-dd" ) );
			return q;
		}

		public static string FormatTorneoIdIn(Liga liga)
		{
			string ids = "'" + string.Join( "','", liga.Torneos.Select( t => t.Id ) ) + "'";
			return string.Format( "and t.id in ({0})", ids );
		}

	}

}