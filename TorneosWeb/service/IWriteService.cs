using Joserras.Commons.Dto;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;

namespace TorneosWeb.service
{
	public interface IWriteService
	{
		Guid UploadTournament(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos);

		void AddPlayer(string nombre);

		JoserrasActionResult DeleteTorneo(Guid torneoId);
	}

}