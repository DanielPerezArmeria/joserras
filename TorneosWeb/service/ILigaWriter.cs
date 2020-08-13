using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.util;

namespace TorneosWeb.service
{
	public interface ILigaWriter
	{
		void InsertarTorneoDeLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos);

		void AgregarNuevaLiga(IFormFile file);

		int AsociarTorneoEnFecha(DateTime date);
	}

}