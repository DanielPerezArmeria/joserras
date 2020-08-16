using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Controllers
{
	public class HomeController : Controller
	{
		private ILigaReader ligaReader;
		private IReadService readService;

		public HomeController(IReadService readService, ILigaReader ligaReader)
		{
			this.readService = readService;
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

		[Route("/LigaActual")]
		public ActionResult LigaActual()
		{
			Liga liga = ligaReader.GetCurrentLiga();
			if( liga == null )
			{
				return RedirectToPage( "/Ligas" );
			}

			return RedirectToPage( "/Liga", "Current" );
		}

		[Route( "/TorneoResults" )]
		public ActionResult TorneoResults(Guid torneoId, string listado)
		{
			Torneo torneo = readService.GetAllTorneos().Where(t=>t.Id == torneoId).First();
			if(listado == "Torneos" )
			{
				return RedirectToPage( "/Torneo", new { id = torneo.Id } );
			}
			else
			{
				return RedirectToPage( "/TorneoLiga", new { torneoId = torneoId } );
			}
		}

	}

}