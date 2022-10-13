using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class TorneoModel : PageModel
	{
		private IReadService readService;
		private IWriteService writeService;
		private ILogger log;

		public Resultados Resultados { get; set; }
		public string Result { get; private set; }

		public TorneoModel(IReadService readService, IWriteService writeService, ILogger<TorneoModel> logger)
		{
			this.readService = readService;
			this.writeService = writeService;
			log = logger;
		}

		public void OnGet(Guid id)
		{
			Resultados = readService.FindResultadosTorneo( id );
		}

		public ActionResult OnPost(Guid torneoId)
		{
			log.LogDebug( "Eliminando torneo {0}", torneoId );

			JoserrasActionResult result = writeService.DeleteTorneo( torneoId );
			if( result.ActionSucceeded )
			{
				return RedirectToPage( "./Torneos" );
			}

			Resultados = readService.FindResultadosTorneo( torneoId );
			Result = result.Description;
			return Page();
		}

	}

}