using Humanizer;
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

		public ClientController(IReadService readService, ILogger<ClientController> log, IFileService tourneyReader,
				IWriteService service)
		{
			this.readService = readService;
			this.log = log;
			this.tourneyReader = tourneyReader;
			writeService = service;
		}

		// GET: api/<ClientController>/Jugadores
		[HttpGet("Jugadores")]
		public IEnumerable<Jugador> GetJugadores()
		{
			log.LogDebug( "Requesting Api: Jugadores" );
			return readService.GetAllJugadores();
		}

		// GET api/<ClientController>/5
		[HttpGet( "{id}" )]
		public string Get(int id)
		{
			return "value";
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
				return BadRequest();
			}

			log.LogDebug( "Api Tournament successfully uploaded!!!!" );
			return Ok();
		}

		// PUT api/<ClientController>/5
		[HttpPut( "{id}" )]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<ClientController>/5
		[HttpDelete( "{id}" )]
		public void Delete(int id)
		{
		}

	}

}