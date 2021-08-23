using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Service.Validators;
using System.Collections.Generic;

namespace Joserras.Client.Torneo.Service
{
	public interface ITorneoValidator
	{
		ValidationResult Validate(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos);
	}

}