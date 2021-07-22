using Joserras.Commons.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class AgregarJugadorModel : PageModel
	{
		private IWriteService writeService;
		public string Result { get; private set; }

		[BindProperty]
		public string Nombre { get; set; }


		public AgregarJugadorModel(IWriteService writeService)
		{
			this.writeService = writeService;
		}

		public ActionResult OnPost()
		{
			if( string.IsNullOrEmpty( Nombre ) )
			{
				Result = "El nombre no puede estar vacío";
				return Page();
			}

			try
			{
				writeService.AddPlayer( Nombre );
			}
			catch(JoserrasException e )
			{
				Result = e.Message;
				return Page();
			}

			return RedirectToPage( "./Jugadores" );
		}

	}

}