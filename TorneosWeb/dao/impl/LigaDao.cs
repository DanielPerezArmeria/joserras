using AutoMapper;
using System;
using System.Data.SqlClient;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;

namespace TorneosWeb.dao.impl
{
	public class LigaDao : ILigaDao
	{
		private JoserrasQuery joserrasQuery;
		private IMapper mapper;

		public LigaDao(JoserrasQuery joserrasQuery, IMapper mapper)
		{
			this.joserrasQuery = joserrasQuery;
			this.mapper = mapper;
		}

		public Liga GetLigaByTorneoId(Guid torneoId)
		{
			string query = string.Format( "select * from ligas where id = (select liga_id from torneos_liga where torneo_id = '{0}')", torneoId );
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

	}

}