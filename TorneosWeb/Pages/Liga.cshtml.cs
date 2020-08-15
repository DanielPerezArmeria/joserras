using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class LigaModel : PageModel
	{
		private ILigaReader ligaReader;

		public Liga Liga { get; set; }
		public List<Standing> Standings { get; set; }

		public LigaModel(ILigaReader ligaReader)
		{
			this.ligaReader = ligaReader;
		}

		public void OnGet(string nombre)
		{
			Liga = ligaReader.FindLigaByNombre( nombre );
		}

		public void OnGetCurrent()
		{
			Liga = ligaReader.GetCurrentLiga();
			if(Liga == null )
			{
				RedirectToPage( "/Ligas" );
			}
			Standings = ligaReader.GetStandings( Liga );
		}

	}

}