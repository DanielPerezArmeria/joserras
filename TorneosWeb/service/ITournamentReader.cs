using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TorneosWeb.service
{
	public interface ITournamentReader
	{
		IEnumerable<T> GetItems<T>(IFormFile file) where T : class;
	}

}