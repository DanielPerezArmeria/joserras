using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

		public ClientController(IReadService readService, ILogger<ClientController> log)
		{
			this.readService = readService;
			this.log = log;
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
		[HttpPost("Upload")]
		public IActionResult Upload(IEnumerable<IFormFile> files)
		{
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