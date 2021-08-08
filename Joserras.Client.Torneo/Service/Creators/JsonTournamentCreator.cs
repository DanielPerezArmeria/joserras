using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Service.Validators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Joserras.Client.Torneo.Service.Creators
{
	public class JsonTournamentCreator : ITournamentCreator
	{
		private readonly ITournamentSender sender;
		private readonly IEnumerable<ITorneoValidator> validators;

		public JsonTournamentCreator(ITournamentSender sender, IEnumerable<ITorneoValidator> validators)
		{
			this.sender = sender;
			this.validators = validators;
		}

		public void CreateTournament(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			string result;

			ValidationResult validationResult = new();
			foreach(ITorneoValidator validator in validators)
			{
				validationResult = validator.Validate( torneo, resultados, kos );
				if (!validationResult.IsValid)
				{
					break;
				}
			}

			if (validationResult.IsValid)
			{
				try
				{
					result = sender.SendTournament( torneo, resultados, kos );
				}
				catch (Exception e)
				{
					result = "No se pudo crear el Torneo." + "\n" + e.Message;
				} 
			}
			else
			{
				result = validationResult.Message;
			}

			MessageBox.Show( result );
		}

		public async Task CreateTournamentAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			ValidationResult validationResult = new();
			foreach (ITorneoValidator validator in validators)
			{
				validationResult = validator.Validate( torneo, resultados, kos );
				if (!validationResult.IsValid)
				{
					break;
				}
			}

			string result = string.Empty;
			if (validationResult.IsValid)
			{
				result = await sender.SendTournamentAsync( torneo, resultados, kos ); 
			}
			else
			{
				result = validationResult.Message;
			}

			MessageBox.Show( result );
		}

	}

}