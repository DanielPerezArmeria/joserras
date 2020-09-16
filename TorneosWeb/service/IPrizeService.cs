using System.Collections.Generic;
using TorneosWeb.domain.dto;

namespace TorneosWeb.service
{
	public interface IPrizeService
	{
		void SetPremiosTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados);
	}

}