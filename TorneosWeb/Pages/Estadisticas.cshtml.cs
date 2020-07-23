using Microsoft.AspNetCore.Mvc.RazorPages;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{
	public class EstadisticasModel : PageModel
	{
		public Estadisticas Estadisticas { get; set; }

		private IStatsService statsService;

		public EstadisticasModel(IStatsService service)
		{
			statsService = service;
		}

		public void OnGet()
		{
			Estadisticas = statsService.GetStats();
		}

	}

}