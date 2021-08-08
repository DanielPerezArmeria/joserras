using Humanizer;
using Joserras.Client.Torneo.Model;
using System.Collections.Generic;
using System.Linq;

namespace Joserras.Client.Torneo.Service.Validators
{
	public class SamePlayerTwiceValidator : ITorneoValidator
	{
		public ValidationResult Validate(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			List<KeyValuePair<string, int>> results = (from r in resultados
																								 group r by r.Jugador into g
																								 select new KeyValuePair<string, int>( g.First().Jugador, g.Count() )).ToList();

			results = results.Where( r => r.Value > 1 ).ToList();

			ValidationResult validationResult = new();
			validationResult.IsValid = true;

			if (results.Count > 0)
			{
				validationResult.IsValid = false;
				validationResult.Message = string.Format( "Los siguiente jugadores tienen más de un registro:\n{0}",
					results.Select( r => r.Key ).Humanize( "y" ) );
			}

			return validationResult;
		}

	}

}
