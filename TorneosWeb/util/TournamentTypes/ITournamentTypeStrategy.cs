using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models;

namespace TorneosWeb.util.TournamentTypes
{
	public interface ITournamentTypeStrategy
	{
		TournamentType GetTournamentType();

		Guid InsertarTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow);
	}

}