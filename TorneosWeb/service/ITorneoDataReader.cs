using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TorneosWeb.service
{
	public interface ITorneoDataReader
	{
		IEnumerable<T> GetItems<T>(IFormFile file) where T : class;
	}

}