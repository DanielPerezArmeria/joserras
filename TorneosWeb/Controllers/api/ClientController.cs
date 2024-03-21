using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.service;

namespace TorneosWeb.Controllers.api
{
	[Route( "api/[controller]" )]
	[ApiController]
	public class ClientController : ControllerBase
	{
		private readonly IReadService readService;
		private ILogger<ClientController> log;
		private IFileService tourneyReader;
		private IWriteService writeService;
		private IPrizeService prizeService;
		private IJugadorService jugadorService;

		public ClientController(IReadService readService, ILogger<ClientController> log, IFileService tourneyReader,
				IWriteService service, IPrizeService prizeService, IJugadorService jugadorService)
		{
			this.readService = readService;
			this.log = log;
			this.tourneyReader = tourneyReader;
			writeService = service;
			this.prizeService = prizeService;
			this.jugadorService = jugadorService;
		}

		// GET: api/<ClientController>/Jugadores
		[HttpGet("Jugadores")]
		public IEnumerable<Jugador> GetJugadores()
		{
			log.LogDebug( "Requesting Api: Jugadores" );
			return jugadorService.GetAllJugadores();
		}

		// GET api/<ClientController>/PrizeRanges
		[HttpGet( "PrizeRanges" )]
		public IEnumerable<PrizeRange> GetPrizeRanges()
		{
			log.LogDebug( "Requesting Api: PrizeRanges" );
			return prizeService.GetPrizeRanges();
		}

		[HttpGet( "GetPrizes/{premiacion}/{buyin}/{liga}/{bolsa}" )]
		public IDictionary<int, string> GetPrizes(string premiacion, int buyin, bool liga, int bolsa)
		{
			log.LogDebug( "Requesting Api: GetPrizes" );
			log.LogDebug( "Premiacion: {0}, Buyin: {1}, Liga:{2}, Bolsa:{3}", premiacion, buyin, liga, bolsa );

			TorneoDTO torneo = new()
			{
				Premiacion = premiacion,
				PrecioBuyin = buyin,
				Liga = liga,
				Bolsa = new Bolsa( bolsa )
			};

			IDictionary<int, string> premios = new Dictionary<int, string>();

			try
			{
				log.LogDebug( "Calling GetTorneos" );
				premios = prizeService.GetPremios( torneo, null );
			}
			catch (Exception e)
			{
				log.LogError( e.Message, e );
			}

			return premios;
		}

		[HttpPost("UploadJson")]
		public IActionResult UploadJson(TorneoUploadWrapper wrapper)
		{
			log.LogDebug( "Post Api: Torneo:'{0}'", wrapper.Torneo );

			try
			{
				writeService.UploadTournament( wrapper.Torneo, wrapper.Resultados, wrapper.Knockouts );
			}
			catch (Exception e)
			{
				log.LogError( e, e.Message );
				log.LogError( "Sending back: " + e.Message );
				return BadRequest( e.Message );
			}

			log.LogDebug( "Api Tournament successfully uploaded!!!!" );
			return Ok();
		}

		// POST api/<ClientController>
		[HttpPost( "UploadFiles" )]
		public IActionResult Upload(List<IFormFile> files)
		{
			log.LogDebug( "Received an UploadFiles request: {0} files.", files.Count );
			try
			{
				TorneoDTO torneo;
				List<ResultadosDTO> resultados;
				List<KnockoutsDTO> kos;

				torneo = tourneyReader.GetFormFileItems<TorneoDTO>( files.Find( t => t.FileName.Contains( "torneo" ) ) ).First();
				resultados = tourneyReader.GetFormFileItems<ResultadosDTO>( files.Find( t => t.FileName.Contains( "resultados" ) ) ).ToList();
				kos = tourneyReader.GetFormFileItems<KnockoutsDTO>( files.Find( t => t.FileName.Contains( "knockouts" ) ) ).ToList();

				writeService.UploadTournament( torneo, resultados, kos );
			}
			catch (Exception e)
			{
				log.LogError( e, e.Message );
				return BadRequest( e.Message );
			}

			log.LogDebug( "Api Tournament successfully uploaded!!!!" );
			return Ok();
		}

	}

}