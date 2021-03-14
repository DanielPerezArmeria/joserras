using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using TorneosWeb.domain.charts;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{

	public class JugadorModel : PageModel
	{
		public DetalleJugador DetalleJugador { get; set; }
		public ProfitChartItem ProfitHistory { get; set; }

		private IReadService readService;
		private IChartService chartService;


		public JugadorModel(IReadService service, IChartService chartService)
		{
			readService = service;
			this.chartService = chartService;
		}

		public void OnGet(Guid id)
		{
			DetalleJugador = readService.FindDetalleJugador( id );
			ProfitHistory = chartService.GetProfitHistoryByPlayerId( id );
		}
			
	}

}