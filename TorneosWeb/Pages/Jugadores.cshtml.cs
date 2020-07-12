using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class JugadoresModel : PageModel
	{
		private IReadService readService;

		public List<Jugador> jugadores { get; set; }

		public JugadoresModel(IReadService service)
		{
			readService = service;
		}

		public void OnGet()
		{
			jugadores = readService.GetAllJugadores();
		}

	}

}