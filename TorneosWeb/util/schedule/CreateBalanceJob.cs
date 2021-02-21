using CronScheduler.Extensions.Scheduler;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.util.schedule
{
	public class CreateBalanceJob : IScheduledJob
	{
		private ILogger<CreateBalanceJob> log;
		private CreateBalanceJobOptions options;
		private IReadService readService;
		private IProfitsExporter exporter;

		public CreateBalanceJob(ILogger<CreateBalanceJob> logger, CreateBalanceJobOptions options, IReadService readService,
			IProfitsExporter profitsExporter)
		{
			log = logger;
			this.options = options;
			this.readService = readService;
			exporter = profitsExporter;
		}

		public string Name => nameof( CreateBalanceJob );

		public Task ExecuteAsync(CancellationToken cancellationToken)
		{
			log.LogDebug( "Creating Balance..." );
			try
			{
				DateTime monday = DateTime.Now.LastWeekMonday();
				DateTime sunday = DateTime.Now.LastWeekSunday();
				List<Torneo> torneos = readService.GetAllTorneos().Where( t => monday <= t.FechaDate && t.FechaDate <= sunday ).ToList();
				exporter.ExportProfits( torneos );
				log.LogDebug( "Balance creado!" );
			}
			catch(Exception e )
			{
				log.LogError( "No se pudo generar el balance" );
				log.LogError( e, e.Message );
			}

			return Task.CompletedTask;
		}

	}

}