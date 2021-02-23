using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.exception;
using TorneosWeb.service;
using TorneosWeb.util;

namespace TorneosWeb.Pages
{
	public class AgregarTorneoModel : PageModel
	{
		private IWriteService writeService;
		private IReadService readService;
		private IProfitsExporter profitsExporter;

		public string Result { get; private set; }
		public string BalanceSheet { get; set; }

		[BindProperty]
		public BufferedMultipleFileUploadPhysical TorneoUpload { get; set; }


		public AgregarTorneoModel(IWriteService service, IReadService readService, IProfitsExporter profitsExporter)
		{
			writeService = service;
			this.readService = readService;
			this.profitsExporter = profitsExporter;

			BalanceSheet = Properties.Resources.BALANCE_SHEET;
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
			catch( JoserrasException je)
			{
				Result = je.Message;
				return Page();
			}

			return RedirectToPage( "./Torneos" );
		}

		public IActionResult OnPostGenerateBalance()
		{
			DateTime monday = DateTime.Now.LastWeekMonday();
			DateTime sunday = DateTime.Now.LastWeekSunday();
			List<Torneo> torneos = readService.GetAllTorneos().Where( t => monday <= t.FechaDate && t.FechaDate <= sunday ).ToList();
			profitsExporter.ExportProfits( torneos );

			Result = "El Balance ha sido creado exitosamente";

			return Page();
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