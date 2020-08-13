using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public abstract class PointRule
	{
		public PointRuleType Type { get; set; }

		public abstract int GetPuntos(Guid jugadorId, Liga liga, Resultados resultados);

		public static Dictionary<string, PointRule> Build(string puntajeLiga)
		{
			string[] reglas = puntajeLiga.Split( ";" );
			Dictionary<string, PointRule> pointRules = new Dictionary<string, PointRule>();
			foreach( string pRule in reglas )
			{
				string[] ruleSplit = pRule.Split( ":" );
				PointRuleType type = (PointRuleType)Enum.Parse( typeof( PointRuleType ), ruleSplit[0] );

				switch( type )
				{
					case PointRuleType.ASISTENCIA:
						pointRules.Add( pRule, new AsistenciaPointRule( ruleSplit[ 1 ] ) );
						break;

					case PointRuleType.POSICION:
						pointRules.Add( pRule, new PosicionPointRule( ruleSplit[ 1 ] ) );
						break;

					case PointRuleType.KO:
						pointRules.Add( pRule, new KosPointRule( ruleSplit[ 1 ] ) );
						break;

					case PointRuleType.PUNTUALIDAD:
						pointRules.Add( pRule, new PuntualidadPointRule( ruleSplit[ 1 ] ) );
						break;

					default:
						break;
				}
			}
			return pointRules;
		}

	}

}