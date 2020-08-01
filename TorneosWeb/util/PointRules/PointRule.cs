using System;
using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public abstract class PointRule
	{
		public PointRuleType Type { get; set; }

		public abstract int GetPuntos(Guid jugadorId, Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos);

		public static PointRule Build(string puntajeLiga, string puntajeTorneo)
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
						pointRules.Add( pRule, new AsistenciaPointRule( ruleSplit ) );
						break;

					case PointRuleType.POSICION:
						pointRules.Add( pRule, new PosicionPointRule( ruleSplit ) );
						break;

					case PointRuleType.KO:
						pointRules.Add( pRule, new KosPointRule( ruleSplit ) );
						break;

					case PointRuleType.PUNTUALIDAD:
						pointRules.Add( pRule, new PuntualidadPointRule( ruleSplit ) );
						break;

					default:
						break;
				}
			}
			return null;
		}

	}

}