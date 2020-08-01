﻿using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.PointRules
{
	public class PosicionPointRule : PointRule
	{
		private int points;
		private int posicion;

		public PosicionPointRule(params string[] Params)
		{
			posicion = int.Parse( Params[ 1 ] );
			points = int.Parse( Params[ 2 ] );
		}

		public override int GetPuntos(Guid jugadorId, Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
		{
			ResultadosDTO res = resultados.Where( p => p.JugadorId == jugadorId ).First();
			if(res.Posicion == posicion )
			{
				return points;
			}

			return 0;
		}

	}

}