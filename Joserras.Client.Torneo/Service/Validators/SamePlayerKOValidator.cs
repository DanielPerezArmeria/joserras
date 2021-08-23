using Humanizer;
using Joserras.Client.Torneo.Model;
using System.Collections.Generic;

namespace Joserras.Client.Torneo.Service.Validators
{
	public class SamePlayerKOValidator : ITorneoValidator
	{
		public ValidationResult Validate(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			ValidationResult validationResult = new();
			List<string> jugadores = new();
			foreach(KO ko in kos)
			{
				if (ko.Jugador.Equals( ko.Eliminado ))
				{
					jugadores.Add( ko.Jugador );
				}
			}

			if(jugadores.Count > 0)
			{
				validationResult.IsValid = false;
				validationResult.Message = string.Format( "Los siguientes jugadores tienen un KO a sí mismos:\n{0}",
					jugadores.Humanize( "y" ) );
			}

			return validationResult;
		}

	}

}