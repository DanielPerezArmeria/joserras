using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class TorneoModel : PageModel
	{
		private IReadService readService;

		public DetalleTorneo DetalleTorneo { get; set; }

		public TorneoModel(IReadService readService)
		{
			this.readService = readService;
		}

		public void OnGet(Guid id)
		{
			DetalleTorneo = readService.GetDetalleTorneo( id );
		}

	}

}