using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class JugadoresModel : PageModel
	{
		private IJugadorService service;

		public List<Jugador> jugadores { get; set; }

		public JugadoresModel(IJugadorService service)
		{
			this.service = service;
		}

		public void OnGet()
		{
			jugadores = service.GetAllJugadores();
		}

	}

}