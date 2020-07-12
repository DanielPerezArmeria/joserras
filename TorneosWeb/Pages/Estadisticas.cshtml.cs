using Microsoft.AspNetCore.Mvc.RazorPages;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class EstadisticasModel : PageModel
	{
		public Estadisticas Estadisticas { get; set; }

		private IReadService readService;

		public EstadisticasModel(IReadService service)
		{
			readService = service;
		}

		public void OnGet()
		{
			Estadisticas = readService.GetStats();
		}

	}

}