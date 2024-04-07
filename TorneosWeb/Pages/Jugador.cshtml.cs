using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Pages
{

	public class JugadorModel : PageModel
	{
		public DetalleJugador DetalleJugador { get; set; }
		public List<JugadorTorneosViewModel> Torneos { get; set; }
		public IDictionary<int, int> Podios { get; set; }
		public string DataPoints { get; set; }

		private IReadService readService;
		private IChartService chartService;
		private IJugadorService jugadorService;

		public JugadorModel(IReadService service, IChartService chartService, IJugadorService jugadorService)
		{
			readService = service;
			this.chartService = chartService;
			this.jugadorService = jugadorService;
		}

		public void OnGet(Guid id)
		{
			JugadorViewModel viewModel = GetJugadorViewModel( id );
			DetalleJugador = viewModel.DetalleJugador;
			Torneos = viewModel.Torneos;
			Podios = viewModel.Podios;
			DataPoints = chartService.GetPlayerProfitChartData( id );
		}

		private JugadorViewModel GetJugadorViewModel(Guid id)
		{
			JugadorViewModel model = new JugadorViewModel();

			TempJugadorModel temp = GetTorneosViewModel( id );
			model.DetalleJugador = readService.FindDetalleJugador( id );
			model.Torneos = temp.Torneos;
			model.Podios = temp.Podios;

			return model;
		}

		private TempJugadorModel GetTorneosViewModel(Guid id)
		{
			List<Posicion> posiciones = jugadorService.GetAllPosicionesByJugador( id );
			List<Torneo> tournaments = readService.GetAllTorneos().Where( t => posiciones.Any( p => p.TorneoId.Equals( t.Id ) ) ).ToList();
			List<JugadorTorneosViewModel> Torneos = new List<JugadorTorneosViewModel>();
			Dictionary<int, int> Podios = new Dictionary<int, int>();

			foreach( Torneo t in tournaments )
			{
				JugadorTorneosViewModel m = new JugadorTorneosViewModel();
				m.Id = t.Id;
				m.Fecha = t.Fecha;
				m.Tipo = t.TipoString;
				m.Ganador = t.Ganador;
				m.GanadorId = t.GanadorId;
				m.Precio_Buyin = t.Precio_Buyin;

				Posicion posicion = posiciones.Single( p => p.TorneoId.Equals( t.Id ) );
				m.Lugar = posicion.Lugar;
				m.Profit = posicion.ProfitTotal;
				m.ProfitNumber = posicion.ProfitTotalNumber;

				if( posicion.Podio )
				{
					if( !Podios.ContainsKey( posicion.Lugar ) )
					{
						Podios.Add( posicion.Lugar, 0 );
					}

					Podios[posicion.Lugar] += 1;
				}

				Torneos.Add( m );
			}

			TempJugadorModel model = new TempJugadorModel();
			model.Torneos = Torneos;
			model.Podios = Podios;

			return model;
		}


		public class JugadorTorneosViewModel
		{
			public Guid Id { get; set; }
			public string Fecha { get; set; }
			public string Tipo { get; set; }
			public string Ganador { get; set; }
			public Guid GanadorId { get; set; }
			[Display( Name = "$ Buyin" )] public string Precio_Buyin { get; set; }
			[Display( Name = "Mi Lugar" )] public int Lugar { get; set; }
			[Display( Name = "Mi Profit" )] public string Profit { get; set; }
			public decimal ProfitNumber { get; set; }
			public Dictionary<int, int> Podios;

		}

		private class TempJugadorModel
		{
			public List<JugadorTorneosViewModel> Torneos;
			public Dictionary<int, int> Podios;
		}

	}

}