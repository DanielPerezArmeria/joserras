using System.Collections.Generic;

namespace Joserras.Commons.Dto
{
	public class TorneoUploadWrapper
	{
		public TorneoDTO Torneo { get; set; }
		public List<ResultadosDTO> Resultados { get; set; }
		public List<KnockoutsDTO> Knockouts { get; set; }

		public TorneoUploadWrapper() { }

		public TorneoUploadWrapper(TorneoDTO Torneo, List<ResultadosDTO> Resultados, List<KnockoutsDTO> Knockouts)
		{
			this.Torneo = Torneo;
			this.Resultados = Resultados;
			this.Knockouts = Knockouts;
		}

	}

}