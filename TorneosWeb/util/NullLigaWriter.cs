using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TorneosWeb.domain.dto;
using TorneosWeb.service;

namespace TorneosWeb.util
{
	public class NullLigaWriter : ILigaWriter
	{
		public void AgregarNuevaLiga(IFormFile file)
		{
			throw new System.NotImplementedException();
		}

		public void InsertarTorneoDeLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow)
		{
			
		}

		public void InsertarTorneoDeLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
		{
			throw new NotImplementedException();
		}

		public int AsociarTorneoEnFecha(DateTime date)
		{
			throw new NotImplementedException();
		}
	}

}