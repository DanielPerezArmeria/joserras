using Microsoft.AspNetCore.Mvc;
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

		public ClientController(IReadService readService)
		{
			this.readService = readService;
		}

		// GET: api/<ClientController>
		[HttpGet("Jugadores")]
		public IEnumerable<Jugador> GetJugadores()
		{
			return readService.GetAllJugadores();
		}

		// GET api/<ClientController>/5
		[HttpGet( "{id}" )]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<ClientController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
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