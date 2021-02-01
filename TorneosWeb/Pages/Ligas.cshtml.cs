using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class LigasModel : PageModel
	{
		private ILigaReader ligaReader;

		[ViewData]
		public Liga LigaAbierta { get; set; }

		public List<Liga> Ligas { get; set; }

		public LigasModel(ILigaReader ligaReader)
		{
			this.ligaReader = ligaReader;
		}

		public void OnGet()
		{
			LigaAbierta = ligaReader.GetCurrentLiga();
			Ligas = ligaReader.GetAllLigas();
		}

	}

}