using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.prize.fillers
{
	#region Floating prize fillers. No afectan el FizedForPercent

	public class PercentPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( PrizeFill.PERCENT_CHAR, "" ) ) / 100;
			decimal otorgado = bolsa.FixedForPercent * factor;
			bolsa.Otorgar( otorgado );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			string[] premios = torneo.Premiacion.Split( PrizeFill.SEPARATOR ); 
			bool notAllContainPercent = premios.Any( r => !r.Contains( PrizeFill.PERCENT_CHAR ) );
			return premio.Contains( PrizeFill.PERCENT_CHAR ) && notAllContainPercent;
		}
	}


	public class AllPercentPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( PrizeFill.PERCENT_CHAR, "" ) ) / 100;
			decimal otorgado = bolsa.Total * factor;
			bolsa.Otorgar( otorgado );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			string[] premios = torneo.Premiacion.Split( PrizeFill.SEPARATOR );
			return premios.All( r => r.Contains( PrizeFill.PERCENT_CHAR ) );
		}
	}

	#endregion


	#region Fixed prize fillers. Sí afectan el Fixed for Percent

	public class SetAmountPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			bolsa.Otorgar( premio.ToDecimal(), true );
			return premio;
		}

		public bool CanHandle(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			return decimal.TryParse( premio, out _ );
		}
	}

	public class PercentAndRemovePrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( PrizeFill.PERCENT_AND_REMOVE, "" ) ) / 100;
			decimal otorgado = bolsa.FixedForPercent * factor;
			bolsa.Otorgar( otorgado, true );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			return premio.Contains( PrizeFill.PERCENT_AND_REMOVE );
		}
	}


	public class FactorPrizeFiller : IPrizeFiller
	{
		private ILigaDao ligaDao;

		public FactorPrizeFiller(ILigaDao ligaDao)
		{
			this.ligaDao = ligaDao;
		}

		public string AssignPrize(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( PrizeFill.FACTOR, "" ) );
			int ligaFee = 0;
			if (torneo.Liga)
			{
				Liga liga = ligaDao.FindCurrentLiga();
				ligaFee = liga == null ? 0 : liga.Fee;
			}
			decimal otorgado = (torneo.PrecioBuyin + ligaFee) * factor;
			bolsa.Otorgar( otorgado, true );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			return premio.Contains( PrizeFill.FACTOR );
		}
	}

	#endregion
}