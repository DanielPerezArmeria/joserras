using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class TorneoModel : PageModel
	{
		private IReadService readService;

		public Resultados Resultados { get; set; }

		public TorneoModel(IReadService readService)
		{
			this.readService = readService;
		}

		public void OnGet(Guid id)
		{
			Resultados = readService.FindResultadosTorneo( id );
		}

	}

}