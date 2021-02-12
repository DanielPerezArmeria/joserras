using Microsoft.AspNetCore.Mvc;
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

		public Liga CurrentLiga { get; set; }

		[ViewData]
		public Liga LigaAbierta { get; set; }

		public LigaModel(ILigaReader ligaReader, IReadService readService, IStatsService statsService)
		{
			this.ligaReader = ligaReader;
			this.readService = readService;
			this.statsService = statsService;
		}

		public void OnGetLiga(string nombre)
		{
			CurrentLiga = ligaReader.FindLigaByNombre( nombre );
			LigaAbierta = ligaReader.GetCurrentLiga();
			if( CurrentLiga == null )
			{
				RedirectToPage( "/Ligas" );
			}
		}

		public void OnGetCurrent()
		{
			CurrentLiga = ligaReader.GetCurrentLiga();
			LigaAbierta = CurrentLiga;
			if(CurrentLiga == null )
			{
				RedirectToPage( "/Ligas" );
			}
		}

	}

}