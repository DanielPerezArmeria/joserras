using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.service;
using TorneosWeb.util.automapper;

namespace TorneosWeb.Pages
{

	public class JugadorModel : PageModel
	{
		public DetalleJugador DetalleJugador { get; set; }
		public List<JugadorTorneosViewModel> Torneos { get; set; }
		public IDictionary<int,int> Podios { get; set; }

		private IReadService readService;
		private IMapper mapper;


		public JugadorModel(IReadService service, IMapper mapper)
		{
			readService = service;
			this.mapper = mapper;
		}

		public void OnGet(Guid id)
		{
			DetalleJugador = readService.FindDetalleJugador( id );
			List<Torneo> tournaments = readService.GetAllTorneos().Where( t => t.Resultados.Posiciones.Any( p => p.JugadorId.Equals( id ) ) ).Take( 15 ).ToList();
			Torneos = new List<JugadorTorneosViewModel>();
			foreach (Torneo t in tournaments)
			{
				JugadorTorneosViewModel m = new JugadorTorneosViewModel();
				m.Id = t.Id;
				m.Fecha = t.Fecha;
				m.Tipo = t.TipoString;
				m.Ganador = t.Ganador;
				m.GanadorId = t.GanadorId;
				m.Precio_Buyin = t.Precio_Buyin;
				m.Lugar = t.Resultados.Posiciones.Single( p => p.JugadorId.Equals( id ) ).Lugar;
				m.Profit = t.Resultados.Posiciones.Single( p => p.JugadorId.Equals( id ) ).ProfitTotal;
				m.ProfitNumber = t.Resultados.Posiciones.Single( p => p.JugadorId.Equals( id ) ).ProfitTotalNumber;

				Torneos.Add( m );
			}

			List <Torneo> torneosConPodio =
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

		}
			
	}

}