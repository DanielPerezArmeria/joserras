using Humanizer;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public abstract class PointRule
	{
		public abstract PointRuleType Type { get; }

		protected int points;

		public int Puntos => points;

		public abstract int GetPuntaje(Guid jugadorId, Liga liga, Resultados resultados);

		public virtual string Descripcion
		{
			get
			{
				return Type.ToString() + ": " + "punto".ToQuantity(Puntos);
			}
		}

		public static IDictionary<string, PointRule> Build(string puntajeLiga)
		{
			string[] reglas = puntajeLiga.Split( ";" );
			SortedDictionary<string, PointRule> pointRules = new SortedDictionary<string, PointRule>();
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