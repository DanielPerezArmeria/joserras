using System;
using System.Collections.Generic;
using TorneosWeb.domain.dto;

namespace TorneosWeb.service
{
	public interface IWriteService
	{
		Guid UploadTournament(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos);

		void AddPlayer(string nombre);
	}

}