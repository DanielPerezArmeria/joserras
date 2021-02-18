using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service
{
	public interface IPrizeService
	{
		void SetPremiosTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados);

		decimal GetBolsaTorneo(int entradas, int buyin, int ligaFee = 0);
	}

}