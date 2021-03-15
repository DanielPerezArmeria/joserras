using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models;

namespace TorneosWeb.service.decorators
{
	public class BalanceGeneratorWriteServiceDecorator : IWriteService
	{
		private IWriteService wrapped;
		private IReadService readService;
		private IProfitsExporter profitsExporter;

		public BalanceGeneratorWriteServiceDecorator(IWriteService wrapped, IReadService readService, IProfitsExporter profitsExporter)
		{
			this.wrapped = wrapped;
			this.readService = readService;
			this.profitsExporter = profitsExporter;
		}

		public void AddPlayer(string nombre)
		{
			wrapped.AddPlayer( nombre );
		}

		public Guid UploadTournament(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
		{
			Guid torneoId = wrapped.UploadTournament( torneo, resultados, kos );

			DayOfWeek dayOfWeek = torneo.Fecha.DayOfWeek;
			if( dayOfWeek == DayOfWeek.Sunday )
			{
				DateTime monday = torneo.Fecha.AddDays( -6 );
				List<Torneo> torneos = readService.GetAllTorneos().Where( t => monday <= t.FechaDate && t.FechaDate <= torneo.Fecha ).ToList();
				profitsExporter.ExportProfits( torneos );
			}

			return torneoId;
		}

	}

}