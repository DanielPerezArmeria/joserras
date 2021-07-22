using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using System.Collections.Generic;

namespace TorneosWeb.service
{
	public interface IPrizeService
	{
		void SetPremiosTorneo(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);

		Bolsa GetBolsaTorneo(int entradas, int rebuys, int buyinPrice, int rebuyPrice);

		string SetPremiacionString(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);
	}

}