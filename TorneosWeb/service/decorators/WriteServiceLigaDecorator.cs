using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.dto;

namespace TorneosWeb.service.decorators
{
	public class WriteServiceLigaDecorator : IWriteService
	{
		private IWriteService wrapped;
		private ILigaReader ligaReader;
		private ILogger<WriteServiceLigaDecorator> log;

		public WriteServiceLigaDecorator(IWriteService wrapped, ILigaReader ligaReader, ILogger<WriteServiceLigaDecorator> logger)
		{
			this.wrapped = wrapped;
			this.ligaReader = ligaReader;
			log = logger;
		}

		public void AddPlayer(string nombre)
		{
			wrapped.AddPlayer( nombre );
		}

		public Guid UploadTournament( TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
		{
			Guid torneoId = wrapped.UploadTournament( torneo, resultados, kos );

			return torneoId;
		}

	}

}