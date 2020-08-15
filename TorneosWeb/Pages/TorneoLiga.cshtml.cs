using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class TorneoLigaModel : PageModel
	{
		public List<Standing> Standings { get; set; }
		private ILigaReader ligaReader;
		private IReadService readService;

		public TorneoLigaModel(IReadService readService, ILigaReader ligaReader)
		{
			this.readService = readService;
			this.ligaReader = ligaReader;
		}

		public void OnGet(Guid torneoId)
		{
			Torneo torneo = readService.GetAllTorneos().Where( t => t.Id == torneoId ).First();
			Standings = ligaReader.GetStandings( torneo.Liga, torneo );
		}

	}

}