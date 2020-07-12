using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{

	public class JugadorModel : PageModel
	{
		public DetalleJugador DetalleJugador { get; set; }

		private IReadService readService;


		public JugadorModel(IReadService service)
		{
			readService = service;
		}

		public void OnGet(Guid id)
		{
			DetalleJugador = readService.GetDetalleJugador( id );
		}
			
	}

}