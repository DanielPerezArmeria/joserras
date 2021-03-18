using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.dto;

namespace TorneosWeb.service
{
	public interface IPrizeService
	{
		void SetPremiosTorneo(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);

		Bolsa GetBolsaTorneo(int entradas, int rebuys, int buyinPrice, int rebuyPrice);

		string SetPremiacionString(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);
	}

}