using Joserras.Client.Torneo.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service
{
	public interface ITournamentSender
	{
		Task<string> SendTournamentAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos);

		string SendTournament(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos);
	}

}