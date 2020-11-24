using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class AgregarLigaModel : PageModel
	{
		private ILigaReader ligaReader;
		private ILigaWriter ligaWriter;

		[ViewData]
		public Liga Liga { get; set; }
		public string Result { get; private set; }

		[BindProperty]
		public BufferedSingleFileUploadPhysical LigaUpload { get; set; }

		[BindProperty]
		public DateTime Fecha { get; set; }


		public AgregarLigaModel(ILigaReader ligaReader, ILigaWriter ligaWriter)
		{
			this.ligaReader = ligaReader;
			this.ligaWriter = ligaWriter;
		}

		public void OnGet()
		{
			Liga = ligaReader.GetCurrentLiga();
		}

		public IActionResult OnPostAgregarLiga()
		{
			if( !ModelState.IsValid )
			{
				Result = "Agrega un archivo";
				return Page();
			}

			ligaWriter.AgregarNuevaLiga( LigaUpload.FormFile );

			Result = null;
			return RedirectToPage( "./AgregarLiga" );
		}

		public IActionResult OnPostCerrarLiga()
		{
			string ligaNombre = Liga.Nombre;
			ligaWriter.CerrarLiga();
			return RedirectToPage( "./Liga", "Liga", new { nombre = ligaNombre } );
		}

		public IActionResult OnPostAgregarTorneo()
		{
			int success = ligaWriter.AsociarTorneoEnFecha( Fecha );
			if(success == 1 )
			{
				Result = string.Format( "El Torneo con fecha {0} se agregó con éxito a la liga!", Fecha.ToShortDateString() );
			}
			else
			{
				Result = string.Format( "El Torneo con fecha {0} no se pudo agregar", Fecha.ToShortDateString() );
			}

			return Page();
		}

	}

	public class BufferedSingleFileUploadPhysical
	{
		[Required]
		[Display( Name = "File" )]
		public IFormFile FormFile { get; set; }

		[Display( Name = "Note" )]
		[StringLength( 50, MinimumLength = 0 )]
		public string Note { get; set; }
	}

}