using System.Collections.Generic;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.service
{
	public interface IPointRuleFactory
	{
		IDictionary<string, PointRule> BuildRules(string ruleString);
	}

}