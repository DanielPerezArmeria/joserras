using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using System.Collections.Generic;

namespace TorneosWeb.util.prize
{
	public interface IPrizeFiller
	{
		bool CanHandle(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio);

		string AssignPrize(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio);
	}

}