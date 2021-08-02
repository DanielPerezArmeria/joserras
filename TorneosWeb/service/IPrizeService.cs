using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using System.Collections.Generic;

namespace TorneosWeb.service
{
	public interface IPrizeService
	{
		IDictionary<int,string> GetPremios(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);

		Bolsa GetBolsaTorneo(int entradas, int rebuys, int buyinPrice, int rebuyPrice);

		string GetPremiacionString(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);

		IEnumerable<PrizeRange> GetPrizeRanges();
	}

}