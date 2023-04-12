using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TorneosWeb.util.csvreader;

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

		public IEnumerable<T> GetFormFileItems<T>(IFormFile file) where T : class
		{
			if(file == null )
			{
				return new List<T>();
			}

			log.LogDebug( "Procesando archivo: {0}", file.FileName );
			
			List<T> items = null;

			CsvConfiguration config = new( CultureInfo.CurrentCulture );
			config.HeaderValidated = null;
			config.MissingFieldFound = null;
			using( StreamReader streamReader = new( file.OpenReadStream() ) )
			{
				CsvReader csv = new( streamReader, config );
				csv.Context.RegisterClassMap<KnockoutsMap>();
				csv.Context.RegisterClassMap<ResultadosMap>();
				try
				{
					items = csv.GetRecords<T>().ToList();
				}
				catch( Exception e )
				{
					string msg = "No se pudo leer el archivo '" + file.FileName + "'. Revisa que esté correcto.";
					throw new Exception( msg, e );
				}
			}

			return items;
		}

	}

}