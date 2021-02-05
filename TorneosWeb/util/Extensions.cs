﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TorneosWeb.util
{
	public static class Extensions
	{
		public static DateTime LastWeekMonday(this DateTime dt)
		{
			DateTime monday;
			if(dt.DayOfWeek == DayOfWeek.Sunday )
			{
				monday = dt.AddDays( -6 );
			}
			else
			{
				int diff = (14 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
				monday = dt.AddDays( -1 * diff ).Date.AddDays(-7);
			}

			return monday;
		}

		public static DateTime LastWeekSunday(this DateTime dt)
		{
			DateTime sunday;
			if( dt.DayOfWeek == DayOfWeek.Sunday )
			{
				sunday = dt;
			}
			else
			{
				int diff = (14 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
				sunday = dt.AddDays( -1 * diff ).Date.AddDays( -1 );
			}

			return sunday;
		}

		public static decimal ToDecimal(this string str)
		{
			return decimal.Parse( str );
		}

	}

}