using System;
using System.Collections.Generic;
using TorneosWeb.util;
using TorneosWeb.util.automapper;
using TorneosWeb.util.PointRules;
using TorneosWeb.util.tiebreakers;

namespace TorneosWeb.domain.models.ligas
{
	public class Liga
	{
		public Guid Id { get; set; }
		public string Nombre { get; set; }

		private string puntaje;
		public string Puntaje
		{
			get { return puntaje; }
			set
			{
				puntaje = value;
				PointRules = PointRule.Build( puntaje );
			}
		}

		public bool Abierta { get; set; }

		[NoMap]
		public string FechaInicio { get; set; }
		public DateTime FechaInicioDate { get; set; }

		[NoMap]
		public string FechaCierre { get; set; }
		public DateTime? FechaCierreDate { get; set; }

		public int Fee { get; set; }

		[NoMap]
		public IDictionary<string, PointRule> PointRules { get; set; }

		[NoMap]
		public List<PointRule> RulesList
		{
			get
			{
				return new List<PointRule>( PointRules.Values );
			}
		}

		[NoMap]
		public List<Torneo> Torneos { get; set; }

		public int GetAcumulado()
		{
			int sum = 0;
			foreach(Torneo t in Torneos )
			{
				sum = sum + (t.Entradas + t.Rebuys) * Fee;
			}
			return sum;
		}

		public string Acumulado
		{
			get
			{
				return GetAcumulado().ToString( Constants.CURRENCY_FORMAT );
			}
		}

		public string Premiacion { get; set; }

		private string desempate;
		public string Desempate
		{
			get { return desempate; }
			set
			{
				desempate = value;
				Tiebreakers = StandingsComparer.Build( desempate );
			}
		}

		[NoMap]
		public Estadisticas Estadisticas { get; set; }


		private List<Standing> standings;
		[NoMap]
		public List<Standing> Standings
		{
			get { return standings; }
			set
			{
				standings = new List<Standing>( value );
				standings.Sort( Tiebreakers );
			}
		}

		[NoMap]
		public AbstractStandingComparer Tiebreakers { get; set; }

	}

}