using AutoMapper;
using Microsoft.Extensions.Configuration;
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


		public LigaReader(IConfiguration conf, IMapper mapper, JoserrasQuery joserrasQuery)
		{
			this.conf = conf;
			this.mapper = mapper;
			this.joserrasQuery = joserrasQuery;
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