using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class LigaReader : ILigaReader
	{
		private IConfiguration conf;
		IMapper mapper;
		JoserrasQuery joserrasQuery;
		ILogger<LigaReader> Log;
		private string ConnString;


		public LigaReader(IConfiguration conf, IMapper mapper, JoserrasQuery joserrasQuery, ILogger<LigaReader> log)
		{
			this.conf = conf;
			this.mapper = mapper;
			this.joserrasQuery = joserrasQuery;
			Log = log;
			ConnString = conf.GetConnectionString( Properties.Resources.joserrasDb );
		}

		public Liga FindLigaByNombre(string nombre)
		{
			throw new NotImplementedException();
		}

		public Liga GetCurrentLiga()
		{
			string query = "select * from ligas where abierta = 1";
			Liga liga = null;
			joserrasQuery.ExecuteQuery( query, reader =>
				{
					while( reader.Read() )
					{
						liga = mapper.Map<SqlDataReader, Liga>( reader );
					}
				} );

			return liga;
		}

		public bool HayLigaAbierta()
		{
			throw new NotImplementedException();
		}
	}

}