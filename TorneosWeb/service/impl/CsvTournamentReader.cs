using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TorneosWeb.service.impl
{
	public class CsvTournamentReader : ITournamentReader
	{
		public IEnumerable<T> GetItems<T>(IFormFile file) where T : class
		{
			if(file == null )
			{
				return new List<T>();
			}
			
			List<T> items = null;

			CsvConfiguration config = new CsvConfiguration( CultureInfo.CurrentCulture );
			config.HeaderValidated = null;
			config.MissingFieldFound = null;
			//config.RegisterClassMap<TorneoMap>();
			using( StreamReader streamReader = new StreamReader( file.OpenReadStream() ) )
			{
				CsvReader csv = new CsvReader( streamReader, config );
				items = csv.GetRecords<T>().ToList();
			}

			return items;
		}

	}

}