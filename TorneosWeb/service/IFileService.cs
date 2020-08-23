using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace TorneosWeb.service
{
	public interface IFileService
	{
		IEnumerable<T> GetFormFileItems<T>(IFormFile file) where T : class;

		void ExportProfits(DateTime start, DateTime end);
	}

}