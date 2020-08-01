using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.service;

namespace TorneosWeb.util
{
	public class NullLigaWriter : ILigaWriter
	{
		public void InsertarLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow)
		{
			
		}

	}

}