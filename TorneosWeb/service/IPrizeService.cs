using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using System.Collections.Generic;

namespace TorneosWeb.service
{
	public interface IPrizeService
	{
		IDictionary<int,string> GetPremios(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);

		Bolsa GetBolsaTorneo(TorneoDTO torneo);

		string GetPremiacionString(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados);

		IEnumerable<PrizeRange> GetPrizeRanges();
	}

}