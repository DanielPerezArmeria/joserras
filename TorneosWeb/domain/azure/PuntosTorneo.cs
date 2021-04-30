using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.dao.azure
{
	public class PuntosTorneo : TableEntity
	{
		public int MyProperty { get; set; }

		public PuntosTorneo() { }

		public PuntosTorneo(string torneoId, string playerId)
		{
			PartitionKey = torneoId;
			RowKey = playerId;
		}

		private SortedDictionary<PointRuleType, FDecimal> standings;
		public SortedDictionary<PointRuleType, FDecimal> Standings
		{
			get { return standings; }
			set
			{
				standings = value;
				if(standings != null)
				{
					StringBuilder builder = new StringBuilder();
					foreach(KeyValuePair<PointRuleType, FDecimal> pair in value)
					{
						builder.Append( string.Format("{0}:{1};", pair.Key.ToString(), pair.Value.ToString() ) );
					}
					puntosReglas = builder.ToString().TrimEnd( ';' );
				}
				else
				{
					puntosReglas = null;
				}
			}
		}

		private string puntosReglas;
		public string PuntosReglas
		{
			get { return puntosReglas; }
			set
			{
				puntosReglas = value;
				standings = new SortedDictionary<PointRuleType, FDecimal>();

				if (!string.IsNullOrEmpty( puntosReglas ))
				{
					string[] dStrings = value.Split( ';' );
					int len = dStrings.Length;

					for(int i = 0; i < len; i++)
					{
						string[] rule = dStrings[i].Split( ":" );
						standings.Add( (PointRuleType)Enum.Parse( typeof( PointRuleType ), rule[0] ), new FDecimal( decimal.Parse( rule[1] ) ) );
					}
				}
			}
		}

	}

}