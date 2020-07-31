using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TorneosWeb.domain.dto;

namespace TorneosWeb.util.TournamentTypes
{
	public interface ITournamentType
	{
		Guid insertarTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados, SqlConnection conn, SqlTransaction tx);
	}

}