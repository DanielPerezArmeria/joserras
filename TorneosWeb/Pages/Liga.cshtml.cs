using Microsoft.AspNetCore.Mvc.RazorPages;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class LigaModel : PageModel
	{
		private ILigaReader ligaReader;
		private IReadService readService;
		private IStatsService statsService;

		public Liga Liga { get; set; }

		public LigaModel(ILigaReader ligaReader, IReadService readService, IStatsService statsService)
		{
			this.ligaReader = ligaReader;
			this.readService = readService;
			this.statsService = statsService;
		}

		public void OnGetLiga(string nombre)
		{
			Liga = ligaReader.FindLigaByNombre( nombre );
			if( Liga == null )
			{
				RedirectToPage( "/Ligas" );
			}
		}

		public void OnGetCurrent()
		{
			Liga = ligaReader.GetCurrentLiga();
			if(Liga == null )
			{
				RedirectToPage( "/Ligas" );
			}
		}

	}

}