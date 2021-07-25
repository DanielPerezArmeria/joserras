using CsvHelper;
using CsvHelper.Configuration;
using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service.Writers
{
	public class CsvTournamentWriter : ITournamentWriter
	{
		public void WriteTournamentFiles(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			string path = torneo.GetPath();

			DirectoryInfo dirInfo = new( path );
			if (!dirInfo.Exists)
			{
				Directory.CreateDirectory( path );
			}

			CsvConfiguration config = new( CultureInfo.CurrentCulture );
			config.MissingFieldFound = null;

			using (StreamWriter writer = new( path + "torneo.csv" ))
			{
				using(CsvWriter csv = new(writer, config ))
				{
					csv.WriteRecords( new List<TorneoViewModel>() { torneo } );
				}
			}

			using (StreamWriter writer = new( path + "resultados.csv" ))
			{
				using (CsvWriter csv = new( writer, config ))
				{
					csv.WriteRecords( resultados );
				}
			}

			if(kos != null && kos.Count > 0)
			{
				using (StreamWriter writer = new( path + "knockouts.csv" ))
				{
					using (CsvWriter csv = new( writer, config ))
					{
						csv.WriteRecords( kos );
					}
				}
			}
		}

		public async Task WriteTournamentFilesAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			throw new NotImplementedException();
		}

	}

}