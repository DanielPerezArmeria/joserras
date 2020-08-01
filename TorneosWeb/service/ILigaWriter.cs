using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.util;

namespace TorneosWeb.service
{
	public interface ILigaWriter
	{
		void InsertarTorneoDeLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow);

		void AgregarNuevaLiga(IFormFile file);
	}

}