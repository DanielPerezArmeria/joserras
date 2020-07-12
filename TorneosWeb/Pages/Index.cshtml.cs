using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class IndexModel : PageModel
	{
		private IReadService readService;

		public List<Torneo> Torneos { get; set; }

		public IndexModel(IReadService readService)
		{
			this.readService = readService;
		}

		public void OnGet()
		{
			Torneos = readService.GetAllTorneos();
		}

	}

}