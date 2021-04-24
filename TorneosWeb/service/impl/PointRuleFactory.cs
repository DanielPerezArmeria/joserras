using SimpleInjector;
using System;
using System.Collections.Generic;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.service.impl
{
	public class PointRuleFactory : IPointRuleFactory
	{
		private readonly Container container;

		public PointRuleFactory(Container container)
		{
			this.container = container;
		}

		public IDictionary<string, PointRule> BuildRules(string puntajeLiga)
		{
			string[] reglas = puntajeLiga.Split( ";" );
			SortedDictionary<string, PointRule> pointRules = new();
			foreach (string pRule in reglas)
			{
				string[] ruleSplit = pRule.Split( ":" );
				PointRuleType type = (PointRuleType)Enum.Parse( typeof( PointRuleType ), ruleSplit[0] );

				switch (type)
				{
					case PointRuleType.ASISTENCIA:
						pointRules.Add( pRule, new AsistenciaPointRule( ruleSplit[1] ) );
						break;

					case PointRuleType.POSICION:
						pointRules.Add( pRule, new PosicionPointRule( ruleSplit[1] ) );
						break;

					case PointRuleType.KO:
						pointRules.Add( pRule, new KosPointRule( ruleSplit[1] ) );
						break;

					case PointRuleType.PUNTUALIDAD:
						pointRules.Add( pRule, new PuntualidadPointRule( ruleSplit[1] ) );
						break;

					case PointRuleType.PEORES:
						pointRules.Add( pRule, new RemoveWorstRule( container.GetInstance<ILigaReader>() ) );
						break;

					default:
						break;
				}
			}
			return pointRules;
		}

	}

}