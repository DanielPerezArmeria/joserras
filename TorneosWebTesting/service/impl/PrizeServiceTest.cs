using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service.impl;
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
		private Mock<ILigaDao> ligaDao;
		private IEnumerable<IPrizeFiller> fillers;

		public PrizeServiceTest()
		{
			logger = new Mock<ILogger<PrizeService>>();
			dao = new Mock<IPrizeDao>();
			ligaDao = new Mock<ILigaDao>();
			fillers = new IPrizeFiller[] {
				new PercentPrizeFiller(),
				new AllPercentPrizeFiller(),
				new PercentAndRemovePrizeFiller(),
				new FactorPrizeFiller( ligaDao.Object ),
				new SetAmountPrizeFiller()
			};

			service = new PrizeService( fillers, dao.Object, logger.Object );
		}

		[Theory]
		[InlineData( "55%-30%-15%", 3, 5225, 2850, 1425 )]
		[InlineData( "50%-50%-1000", 3, 4250, 4250, 1000 )]
		[InlineData( "50%-50%-2x", 3, 4250, 4250, 1000 )]
		[InlineData( "55%-45%-15&", 3, 4441.25, 3633.75, 1425 )]
		public void TestGetPremios(string premioString, int expectedPrizes, decimal first, decimal second, decimal third )
		{
			TorneoDTO torneo = CreateTorneo( 11, 8, 500, false );
			torneo.Premiacion = premioString;

			IDictionary<int,string> premios = service.GetPremios( torneo, null );

			Assert.Equal( expectedPrizes, premios.Count );

			Assert.Equal( first, decimal.Parse( premios[1] ) );
			Assert.Equal( second, decimal.Parse( premios[2] ) );
			Assert.Equal( third, decimal.Parse( premios[3] ) );
		}

		[Theory]
		[InlineData( "55%-30%-15%-2x", 4, 4510, 2460, 1230, 1300 )]
		[InlineData( "50%-50%-15&-2x", 4, 3485, 3485, 1230, 1300 )]
		public void TestGetPremiosLiga(string premioString, int expectedPrizes, decimal first, decimal second,
				decimal third, decimal fourth)
		{
			Liga liga = new();
			liga.Fee = 150;
			ligaDao.Setup( d => d.FindCurrentLiga() ).Returns( liga );

			TorneoDTO torneo = CreateTorneo( 11, 8, 500, true );
			torneo.Premiacion = premioString;

			IDictionary<int, string> premios = service.GetPremios( torneo, null );

			Assert.Equal( expectedPrizes, premios.Count );

			Assert.Equal( first, decimal.Parse( premios[1] ) );
			Assert.Equal( second, decimal.Parse( premios[2] ) );
			Assert.Equal( third, decimal.Parse( premios[3] ) );
			Assert.Equal( fourth, decimal.Parse( premios[4] ) );
		}


		private TorneoDTO CreateTorneo(int entradas, int rebuys, int buyin, bool isLiga)
		{
			TorneoDTO torneo = new();
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