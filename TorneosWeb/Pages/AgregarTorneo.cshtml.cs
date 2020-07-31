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

		public AgregarTorneoModel(IWriteService service)
		{
			writeService = service;
		}

		[BindProperty]
		public BufferedMultipleFileUploadPhysical FileUpload { get; set; }

		public void OnGet()
		{

		}

		public IActionResult OnPost()
		{
			try
			{
				writeService.uploadTournament( FileUpload.FormFiles );
			}
			catch( JoserrasException )
			{
				throw;
			}

			return RedirectToPage( "./Index" );
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