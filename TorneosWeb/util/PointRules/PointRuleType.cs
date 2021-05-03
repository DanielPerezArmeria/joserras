namespace TorneosWeb.util.PointRules
{
	public enum PointRuleType
	{
		POSICION,
		KO,
		REBUY,
		ASISTENCIA,
		PUNTUALIDAD,
		PEOR
	}

	public static class PointRuleExtensions
	{
		public static RuleScope GetScope(this PointRuleType type)
		{
			switch (type)
			{
				case PointRuleType.PEOR:
					return RuleScope.LIGA;
				default:
					return RuleScope.TORNEO;
			}

		}

	}

}