using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;

namespace TorneosWeb.util.PointRules
{
	public class RemoveWorstRule : PointRule
	{
		private readonly Container container;

		public override PointRuleType Type => PointRuleType.PEOR;

		public override RuleScope RuleScope => RuleScope.LIGA;

		public RemoveWorstRule(Container container)
		{
			this.container = container;
		}

		public override decimal GetPuntaje(Guid jugadorId, Liga liga, Resultados resultados)
		{
			ILigaReader ligaReader = container.GetInstance<ILigaReader>();

			int numTorneos = liga.Torneos.Count;
			if(numTorneos < 2)
			{
				return 0;
			}

			List<Torneo> torneosParticipados =
				liga.Torneos.Where( t => t.Resultados.Posiciones.Any( p => p.JugadorId == jugadorId ) ).ToList();

			if(torneosParticipados.Count < numTorneos)
			{
				return 0;
			}

			decimal menor = ligaReader.GetStandings( liga, torneosParticipados.ElementAt( 0 ) )
					.Single(s=>s.JugadorId == jugadorId).Total;
			foreach(Torneo torneo in torneosParticipados)
			{
				points = ligaReader.GetStandings( liga, torneo ).Single( s => s.JugadorId == jugadorId ).Total;
				if(points < menor)
				{
					menor = points;
				}
			}

			return menor * -1;
		}

		public override string Descripcion
		{
			get
			{
				return Type.ToString() + ": No cuenta el peor resultado.";
			}
		}

	}

}