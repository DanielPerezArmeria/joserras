using Joserras.Client.Torneo.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Joserras.Client.Torneo.Service.Creators
{
	public class SaveFilesTournamentCreator : ITournamentCreator
	{
		private ITournamentWriter writer;
		private ITournamentSender sender;

		public SaveFilesTournamentCreator(ITournamentWriter writer, ITournamentSender sender)
		{
			this.writer = writer;
			this.sender = sender;
		}

		public void CreateTournament(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			string result;
			try
			{
				writer.WriteTournamentFiles( torneo, resultados, kos );
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
			bool filesCreated = false;
			string result;
			try
			{
				writer.WriteTournamentFiles( torneo, resultados, kos );
				filesCreated = true;
				result = "Archivos creados";
			}
			catch (Exception e)
			{
				result = "No se pudo crear el Torneo." + "\n" + e.Message;
			}

			if (filesCreated)
			{
				result = await sender.SendTournamentAsync( torneo, resultados, kos );
			}

			MessageBox.Show( result );
		}

	}

}