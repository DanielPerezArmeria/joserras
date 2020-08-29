using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TorneosWeb.domain.models;

namespace TorneosWeb.service.impl
{
	public class FileService : IFileService
	{
		private IReadService readService;

		public FileService(IReadService readService)
		{
			this.readService = readService;
		}

		public void ExportProfits(DateTime start, DateTime end)
		{
			List<DetalleJugador> detalles = readService.GetAllDetalleJugador( start, end );
		}

		public IEnumerable<T> GetFormFileItems<T>(IFormFile file) where T : class
		{
			if(file == null )
			{
				return new List<T>();
			}
			
			List<T> items = null;

			CsvConfiguration config = new CsvConfiguration( CultureInfo.CurrentCulture );
			config.HeaderValidated = null;
			config.MissingFieldFound = null;
			using( StreamReader streamReader = new StreamReader( file.OpenReadStream() ) )
			{
				CsvReader csv = new CsvReader( streamReader, config );
				items = csv.GetRecords<T>().ToList();
			}

			return items;
		}

	}

}