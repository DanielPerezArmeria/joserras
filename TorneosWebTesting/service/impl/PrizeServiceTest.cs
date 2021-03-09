using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TorneosWeb.dao;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.dto;
using TorneosWeb.service.impl;
using TorneosWeb.util;
using TorneosWeb.util.prize;
using TorneosWeb.util.prize.fillers;
using Xunit;

namespace TorneosWebTesting.service.impl
{
	public class PrizeServiceTest
	{
		private PrizeService service;

		private Mock<ILogger<PrizeService>> logger;
		private Mock<IPrizeDao> dao;
		private IEnumerable<IPrizeFiller> fillers;

		public PrizeServiceTest()
		{
			logger = new Mock<ILogger<PrizeService>>();
			dao = new Mock<IPrizeDao>();
			fillers = new IPrizeFiller[] {
				new SinglePercentPrizeFiller(),
				new AllPercentPrizeFiller(),
				new FixedPercentPrizeFiller(),
				new FactorPrizeFiller(),
				new SetAmountPrizeFiller()
			};

			service = new PrizeService( fillers, dao.Object, logger.Object );
		}

		[Fact]
		public void TestPrizesDao()
		{
			dao.Setup( d => d.GetPrizeRanges() ).Returns( CreatePrizeRange( "55%-30%-15%" ) );

			TorneoDTO torneo = CreateTorneo( 11, 8, 500, false );
			IEnumerable<ResultadosDTO> resultados = CreateResultados( 11, 8 );

			Assert.Equal( 8, resultados.Sum( r => r.Rebuys ) );

			service.SetPremiosTorneo( torneo, resultados );

			Assert.Equal( 3, resultados.Where( r => !string.IsNullOrEmpty( r.Premio ) ).Count() );

			Assert.Equal( 5225m, resultados.Single( r => r.Posicion == 1 ).Premio.ToDecimal() );
			Assert.Equal( 1425m, resultados.Single( r => r.Posicion == 3 ).Premio.ToDecimal() );
		}

		[Fact]
		public void TestPremiosInResultados()
		{
			TorneoDTO torneo = CreateTorneo( 11, 9, 500, false );
			IEnumerable<ResultadosDTO> resultados = CreateResultados( 11, 9 );

			resultados.Single( r => r.Posicion == 1 ).Premio = "50%";
			resultados.Single( r => r.Posicion == 2 ).Premio = "50%";
			resultados.Single( r => r.Posicion == 3 ).Premio = "1000";

			service.SetPremiosTorneo( torneo, resultados );

			Assert.Equal( 3, resultados.Where( r => !string.IsNullOrEmpty( r.Premio ) ).Count() );

			Assert.Equal( 4500m, resultados.Single( r => r.Posicion == 1 ).Premio.ToDecimal() );
			Assert.Equal( 4500m, resultados.Single( r => r.Posicion == 2 ).Premio.ToDecimal() );
			Assert.Equal( 1000m, resultados.Single( r => r.Posicion == 3 ).Premio.ToDecimal() );
		}

		[Fact]
		public void TestPrizes2()
		{
			TorneoDTO torneo = CreateTorneo( 11, 9, 500, false );
			torneo.Premiacion = "55%-45%-15p-2x";
			IEnumerable<ResultadosDTO> resultados = CreateResultados( 11, 9 );

			Assert.Equal( 9, resultados.Sum( r => r.Rebuys ) );

			service.SetPremiosTorneo( torneo, resultados );

			Assert.Equal( 4, resultados.Where( r => !string.IsNullOrEmpty( r.Premio ) ).Count() );

			Assert.Equal( 4207.5m, resultados.Single( r => r.Posicion == 1 ).Premio.ToDecimal() );
			Assert.Equal( 3442.5m, resultados.Single( r => r.Posicion == 2 ).Premio.ToDecimal() );
			Assert.Equal( 1350m, resultados.Single( r => r.Posicion == 3 ).Premio.ToDecimal() );
			Assert.Equal( 1000m, resultados.Single( r => r.Posicion == 4 ).Premio.ToDecimal() );
		}

		[Fact]
		public void TestPrizes3()
		{
			TorneoDTO torneo = CreateTorneo( 11, 9, 500, false );
			torneo.Premiacion = "55%-30%-15%-2x";
			IEnumerable<ResultadosDTO> resultados = CreateResultados( 11, 9 );

			Assert.Equal( 9, resultados.Sum( r => r.Rebuys ) );

			service.SetPremiosTorneo( torneo, resultados );

			Assert.Equal( 4, resultados.Where( r => !string.IsNullOrEmpty( r.Premio ) ).Count() );

			Assert.Equal( 4950m, resultados.Single( r => r.Posicion == 1 ).Premio.ToDecimal() );
			Assert.Equal( 2700m, resultados.Single( r => r.Posicion == 2 ).Premio.ToDecimal() );
			Assert.Equal( 1350m, resultados.Single( r => r.Posicion == 3 ).Premio.ToDecimal() );
			Assert.Equal( 1000m, resultados.Single( r => r.Posicion == 4 ).Premio.ToDecimal() );
		}


		private TorneoDTO CreateTorneo(int entradas, int rebuys, int buyin, bool isLiga)
		{
			TorneoDTO torneo = new TorneoDTO();
			torneo.Liga = isLiga;
			torneo.PrecioBuyin = buyin;
			torneo.Fecha = DateTime.Now;
			torneo.Tipo = TournamentType.NORMAL;
			torneo.Entradas = entradas;
			torneo.Rebuys = rebuys;
			torneo.Bolsa = new Bolsa( (entradas + rebuys) * buyin );
			return torneo;
		}

		private List<ResultadosDTO> CreateResultados(int entradas, int rebuys)
		{
			List<ResultadosDTO> resultados = new List<ResultadosDTO>();
			for(int i = 1;  i <= entradas; i++ )
			{
				ResultadosDTO res = new ResultadosDTO();
				res.Jugador = RandomString( 10 );
				res.Posicion = i;
				res.Rebuys = rebuys > 0 ? random.Next( 0, rebuys ) : 0;
				rebuys -= res.Rebuys;

				resultados.Add( res );
			}

			if(rebuys > 0 )
			{
				resultados.Last().Rebuys += rebuys;
			}

			return resultados;
		}

		private IEnumerable<PrizeRange> CreatePrizeRange(string premiacion)
		{
			PrizeRange range = new PrizeRange( 11, 26, premiacion );
			return new PrizeRange[] { range };
		}

		private static Random random = new Random();
		private string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string( Enumerable.Repeat( chars, length )
				.Select( s => s[ random.Next( s.Length ) ] ).ToArray() );
		}

	}

}