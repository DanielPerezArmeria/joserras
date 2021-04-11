using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
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
		public List<Torneo> Torneos { get; set; }
		public IDictionary<int,int> Podios { get; set; }

		private IReadService readService;


		public JugadorModel(IReadService service)
		{
			readService = service;
		}

		public void OnGet(Guid id)
		{
			DetalleJugador = readService.FindDetalleJugador( id );
			Torneos = readService.GetAllTorneos().Where( t => t.Resultados.Posiciones.Any( p => p.JugadorId.Equals( id ) ) ).Take( 15 ).ToList();

			List<Torneo> torneosConPodio =
				readService.GetAllTorneos().Where( t => t.Resultados.Posiciones.Any( p => p.JugadorId.Equals( id ) && p.Podio ) ).ToList();

			Podios = new Dictionary<int, int>();
			foreach(Torneo t in torneosConPodio )
			{
				Posicion pos = t.Resultados.Posiciones.Single( p => p.JugadorId.Equals( id ) );
				if( !Podios.ContainsKey( pos.Lugar ) )
				{
					Podios.Add( pos.Lugar, 0 );
				}

				Podios[ pos.Lugar ] += 1;
			}
		}
			
	}

}