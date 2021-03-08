using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.dto;

namespace TorneosWeb.util.prize.fillers
{
	#region Floating prize fillers. No afectan el FizedForPercent

	public class SinglePercentPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( PrizeFill.PERCENT_CHAR, "" ) ) / 100;
			decimal otorgado = bolsa.FixedForPercent * factor;
			bolsa.Otorgar( otorgado );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			bool notAllContainPercent = resultados.Any( r => !r.Premio.Contains( PrizeFill.PERCENT_CHAR ) );
			return premio.Contains( PrizeFill.PERCENT_CHAR ) && notAllContainPercent;
		}
	}


	public class AllPercentPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( PrizeFill.PERCENT_CHAR, "" ) ) / 100;
			decimal otorgado = bolsa.Total * factor;
			bolsa.Otorgar( otorgado );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			return resultados.All( r => r.Premio.Contains( PrizeFill.PERCENT_CHAR ) );
		}
	}

	#endregion


	#region Fixed prize fillers. Sí afectan el Fixed for Percent

	public class FixedPercentPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( PrizeFill.FIXED_PERCENT, "" ) ) / 100;
			decimal otorgado = bolsa.FixedForPercent * factor;
			bolsa.Otorgar( otorgado, true );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			return premio.Contains( PrizeFill.FIXED_PERCENT );
		}
	}


	public class FactorPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			decimal factor = decimal.Parse( premio.Replace( "x", "" ) );
			decimal otorgado = torneo.PrecioBuyin * factor;
			bolsa.Otorgar( otorgado, true );
			return otorgado.ToString();
		}

		public bool CanHandle(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			return premio.Contains( PrizeFill.FACTOR );
		}
	}


	public class AllSetPrizeFiller : IPrizeFiller
	{
		public string AssignPrize(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			bolsa.Otorgar( premio.ToDecimal(), true );
			return premio;
		}

		public bool CanHandle(TorneoDTO torneo, List<ResultadosDTO> resultados, Bolsa bolsa, string premio)
		{
			return resultados.All( r => decimal.TryParse( r.Premio, out decimal d ) );
		}
	}

	#endregion
}