using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.dto;

namespace TorneosWeb.service
{
	public interface IPrizeService
	{
		void SetPremiosTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados);

		Bolsa GetBolsaTorneo(int entradas, int buyin, int ligaFee = 0);
	}

}