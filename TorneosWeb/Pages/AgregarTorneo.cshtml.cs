using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TorneosWeb.exception;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class AgregarTorneoModel : PageModel
	{
		private IWriteService writeService;

		public string Result { get; private set; }

		[BindProperty]
		public BufferedMultipleFileUploadPhysical TorneoUpload { get; set; }


		public AgregarTorneoModel(IWriteService service)
		{
			writeService = service;
		}

		public void OnGet()
		{

		}

		public IActionResult OnPost()
		{
			if( !ModelState.IsValid )
			{
				Result = "Please correct the form.";
				return Page();
			}

			try
			{
				writeService.uploadTournament( TorneoUpload.FormFiles );
			}
			catch( JoserrasException )
			{
				throw;
			}

			return RedirectToPage( "./Torneos" );
		}

	}

	public class BufferedMultipleFileUploadPhysical
	{
		[Required]
		[Display( Name = "File" )]
		public List<IFormFile> FormFiles { get; set; }

		[Display( Name = "Note" )]
		[StringLength( 50, MinimumLength = 0 )]
		public string Note { get; set; }
	}

}