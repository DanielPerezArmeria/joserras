using Joserras.Client.Torneo.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Joserras.Client.Torneo.Service.Creators
{
	public class JsonTournamentCreator : ITournamentCreator
	{
		private ITournamentSender sender;

		public JsonTournamentCreator(ITournamentSender sender)
		{
			this.sender = sender;
		}

		public void CreateTournament(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			string result;
			try
			{
				result = sender.SendTournament( torneo, resultados, kos );
			}
			catch (Exception e)
			{
				result = "No se pudo crear el Torneo." + "\n" + e.Message;
			}

			MessageBox.Show( result );
		}

		public async Task CreateTournamentAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			string result = await sender.SendTournamentAsync( torneo, resultados, kos );

			MessageBox.Show( result );
		}

	}

}