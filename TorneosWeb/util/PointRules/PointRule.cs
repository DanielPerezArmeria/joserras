using Humanizer;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public abstract class PointRule
	{
		public abstract PointRuleType Type { get; }

		protected decimal points;

		public decimal Puntos => points;

		public abstract decimal GetPuntaje(Guid jugadorId, Liga liga, Resultados resultados);

		public virtual RuleScope RuleScope
		{
			get { return RuleScope.TORNEO; }
		}

		public virtual string Descripcion
		{
			get
			{
				return Type.ToString() + ": " + "punto".ToQuantity( Convert.ToInt32( decimal.Ceiling( Puntos ) ) );
			}
		}

	}

}