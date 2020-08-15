using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class LigasModel : PageModel
	{
		private ILigaReader ligaReader;

		public List<Liga> Ligas { get; set; }

		public LigasModel(ILigaReader ligaReader)
		{
			this.ligaReader = ligaReader;
		}

		public void OnGet()
		{
			Ligas = ligaReader.GetAllLigas();
		}

	}

}