using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models;

namespace TorneosWeb.util.TournamentTypes
{
	public class NormalTournamentTypeStrategy : ITournamentTypeStrategy
	{
		public TournamentType GetTournamentType()
		{
			return TournamentType.NORMAL;
		}

		public Guid InsertarTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			int rebuys = resultados.Sum( d => d.Rebuys );
			int bolsa = (torneo.PrecioBuyin * resultados.Count) + (torneo.PrecioRebuy * rebuys);

			Guid torneoId = Guid.Parse( uow.ExecuteScalar( Properties.Queries.InsertTorneo, torneo.Fecha.ToString( "yyyy-MM-dd" ),
					torneo.PrecioBuyin, torneo.PrecioRebuy, resultados.Count, rebuys, bolsa, torneo.Tipo.ToString(), torneo.PrecioBounty )
					.ToString() );

			return torneoId;
		}

	}

}