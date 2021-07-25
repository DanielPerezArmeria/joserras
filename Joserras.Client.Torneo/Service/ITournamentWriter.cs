using Joserras.Client.Torneo.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service
{
	public interface ITournamentWriter
	{
		void WriteTournamentFiles(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos);

		Task WriteTournamentFilesAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos);
	}

}