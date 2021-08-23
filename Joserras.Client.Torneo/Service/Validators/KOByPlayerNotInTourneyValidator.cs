using Humanizer;
using Joserras.Client.Torneo.Model;
using System.Collections.Generic;

namespace Joserras.Client.Torneo.Service.Validators
{
	public class KOByPlayerNotInTourneyValidator : ITorneoValidator
	{
		public ValidationResult Validate(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			ValidationResult validationResult = new();
			HashSet<string> jugadores = new();

			foreach(KO ko in kos)
			{
				if(!resultados.Exists( r => ko.Jugador.Equals( r.Jugador ) ) ){
					jugadores.Add( ko.Jugador );
				}
				if (!resultados.Exists( r => ko.Eliminado.Equals( r.Jugador ) ) ){
					jugadores.Add( ko.Eliminado );
				}
			}

			if(jugadores.Count > 0)
			{
				validationResult.IsValid = false;
				validationResult.Message = string.Format( "Los siguientes jugadores con KO's no están registrados en el torneo:\n{0}",
					jugadores.Humanize( "y" ) );
			}

			return validationResult;
		}

	}

}