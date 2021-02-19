using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
		private ILogger<FileService> log;

		public FileService(IReadService readService, ILogger<FileService> logger)
		{
			this.readService = readService;
			this.log = logger;
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

			log.LogDebug( "Procesando archivo: {0}", file.FileName );
			
			List<T> items = null;

			CsvConfiguration config = new CsvConfiguration( CultureInfo.CurrentCulture );
			config.HeaderValidated = null;
			config.MissingFieldFound = null;
			using( StreamReader streamReader = new StreamReader( file.OpenReadStream() ) )
			{
				CsvReader csv = new CsvReader( streamReader, config );
				try
				{
					items = csv.GetRecords<T>().ToList();
				}
				catch( Exception e )
				{
					log.LogError( "Unable to read file " + file.FileName, e );
					throw;
				}
			}

			return items;
		}

	}

}