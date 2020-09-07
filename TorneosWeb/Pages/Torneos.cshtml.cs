using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class TorneosModel : PageModel
	{
		private IReadService readService;

		public List<Torneo> Torneos { get; set; }

		public TorneosModel(IReadService readService)
		{
			this.readService = readService;
		}

		public void OnGet()
		{
			Torneos = readService.GetAllTorneos().Take( 15 ).ToList();
		}

	}

}