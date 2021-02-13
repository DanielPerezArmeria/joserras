using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.models;
using TorneosWeb.service;
using TorneosWeb.service.impl;
using Xunit;

namespace TorneosWebTesting.service.impl
{
	public class GoogleSheetsProfitsExporterTest
	{
		private GoogleSheetsProfitsExporter exporter;

		private Mock<ILogger<GoogleSheetsProfitsExporter>> logger;
		private Mock<IReadService> readService;

		[Fact]
		public void ExportProfitsTest()
		{
			logger = new Mock<ILogger<GoogleSheetsProfitsExporter>>();
			readService = new Mock<IReadService>();

			List<Torneo> torneos = CreateTorneos();

			readService.SetupSequence( f => f.FindResultadosTorneo( It.IsAny<Guid>() ) )
				.Returns( CreateResultadosA() )
				.Returns( CreateResultadosB() );

			readService.Setup( f => f.GetAllJugadores() ).Returns( CreateJugadores() );

			exporter = new GoogleSheetsProfitsExporter( @"./Files/Joserras Project-fd7d4368e5dd.json",
				"1fWhxbneW19urTN7RFMaTRtycIB32X6C4LPM9QGzDY-w", logger.Object, readService.Object );

			exporter.ExportProfits( torneos );
		}

		private List<Torneo> CreateTorneos()
		{
			List<Torneo> torneos = new List<Torneo>();

			Torneo torneo = new Torneo();
			torneo.Id = Guid.NewGuid();
			torneo.FechaDate = new DateTime( 2020, 10, 10 );
			torneo.PrecioBuyinNumber = 500;
			torneo.PrecioRebuyNumber = 500;
			torneos.Add( torneo );

			torneo = new Torneo();
			torneo.Id = Guid.NewGuid();
			torneo.FechaDate = new DateTime( 2020, 10, 05 );
			torneo.PrecioBuyinNumber = 500;
			torneo.PrecioRebuyNumber = 500;
			torneos.Add( torneo );

			return torneos;
		}

		private Resultados CreateResultadosA()
		{
			Resultados resultados = new Resultados();

			List<Posicion> posiciones = new List<Posicion>();
			posiciones.Add( CreatePosicion( "Daniel", 1000) );
			posiciones.Add( CreatePosicion( "Mote", 500 ) );
			posiciones.Add( CreatePosicion( "Korean", -300 ) );
			posiciones.Add( CreatePosicion( "Gers", 77 ) );

			resultados.Posiciones = posiciones;
			return resultados;
		}

		private Resultados CreateResultadosB()
		{
			Resultados resultados = new Resultados();

			List<Posicion> posiciones = new List<Posicion>();
			posiciones.Add( CreatePosicion( "Daniel", 250 ) );
			posiciones.Add( CreatePosicion( "Mote", -600 ) );
			posiciones.Add( CreatePosicion( "Korean", -500 ) );

			resultados.Posiciones = posiciones;
			return resultados;
		}

		private Posicion CreatePosicion(string nombre, int profit)
		{
			return new Posicion { Nombre = nombre, ProfitNumber = profit };
		}

		private List<Jugador> CreateJugadores()
		{
			List<Jugador> jugadores = new List<Jugador>();
			jugadores.Add( new Jugador { Nombre = "Daniel" } );
			jugadores.Add( new Jugador { Nombre = "Mote" } );
			jugadores.Add( new Jugador { Nombre = "Korean" } );
			jugadores.Add( new Jugador { Nombre = "Gers" } );

			return jugadores;
		}

	}

}