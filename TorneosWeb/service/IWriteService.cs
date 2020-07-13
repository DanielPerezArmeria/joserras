using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TorneosWeb.service
{
	public interface IWriteService
	{
		void uploadTournament(List<IFormFile> torneos);
	}

}