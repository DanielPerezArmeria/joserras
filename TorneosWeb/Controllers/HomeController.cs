using Microsoft.AspNetCore.Mvc;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Controllers
{
	public class HomeController : Controller
	{
		private ILigaReader ligaReader;

		public HomeController(ILigaReader ligaReader)
		{
			this.ligaReader = ligaReader;
		}
		
		[Route("/")]
		public ActionResult Index()
		{
			Liga liga = ligaReader.GetCurrentLiga();
			if(liga == null )
			{
				return RedirectToPage( "/Torneos" );
			}

			return RedirectToPage( "/Liga", "Current" );
		}

	}

}