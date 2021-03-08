using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.dto;

namespace TorneosWeb.util.prize
{
	public interface IPrizeFiller
	{
		bool CanHandle(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio);

		string AssignPrize(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio);
	}

}