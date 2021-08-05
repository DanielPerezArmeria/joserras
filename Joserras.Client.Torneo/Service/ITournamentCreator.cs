using Joserras.Client.Torneo.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service
{
	public interface ITournamentCreator
	{
		Task CreateTournamentAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos);

		void CreateTournament(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos);
	}

}