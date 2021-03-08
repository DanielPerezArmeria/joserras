using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models.dto;
using TorneosWeb.exception;
using TorneosWeb.util;
using TorneosWeb.util.prize;

namespace TorneosWeb.service.impl
{
	public class PrizeService : IPrizeService
	{
		private ILogger<PrizeService> log;
		private IPrizeDao prizeDao;
		private IEnumerable<IPrizeFiller> fillers;

		public PrizeService(IEnumerable<IPrizeFiller> fillers, IPrizeDao dao, ILogger<PrizeService> logger)
		{
			prizeDao = dao;
			log = logger;
			this.fillers = fillers;
		}

		public void SetPremiosTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados)
		{
			int entradas = torneo.Entradas + torneo.Rebuys;
			IEnumerable<PrizeRange> PrizeRanges = prizeDao.GetPrizeRanges();
			PrizeRange selectedRange = PrizeRanges.First( r => r.IsBetween( entradas ) );
			log.LogDebug( "Selected prize range: {0}", selectedRange.ToString() );

			FillPrizes( torneo, resultados, selectedRange, torneo.Bolsa );
		}

		private void FillPrizes(TorneoDTO torneo, List<ResultadosDTO> resultados, PrizeRange selectedRange, Bolsa bolsa)
		{
			IEnumerable<string> premios = string.IsNullOrEmpty( torneo.Premiacion ) ?
					selectedRange.Premiacion.Split( "-" ) : torneo.Premiacion.Split( "-" );

			int placesAwarded = premios.Count();
			for( int i = placesAwarded; i > 0; i-- )
			{
				try
				{
					ResultadosDTO res = resultados.First( r => r.Posicion == i );
					string premio = res.Premio.IsNullEmptyOrZero() ? premios.ElementAt( i - 1 ) : res.Premio;
					decimal otorgado = 0;
					if( premio.Contains( '%' ) )
					{
						decimal factor = decimal.Parse( premio.Replace( "%", "" ) ) / 100;
						otorgado = bolsa.Total * factor;
					}
					else if( premio.Contains( '&' ) )
					{
						decimal factor = decimal.Parse( premio.Replace( "&", "" ) ) / 100;
						otorgado = bolsa.Remanente * factor;
					}
					else if( premio.Contains( 'p' ) )
					{
						decimal factor = decimal.Parse( premio.Replace( "p", "" ) ) / 100;
						otorgado = bolsa.Remanente * factor;
						bolsa.Remanente -= otorgado;
					}
					else if( premio.Contains( 'x' ) )
					{
						decimal factor = decimal.Parse( premio.Replace( "x", "" ) );
						otorgado = torneo.PrecioBuyin * factor;
						bolsa.Remanente -= otorgado;
					}
					else
					{
						otorgado = premio.ToDecimal();
						bolsa.Remanente -= otorgado;
					}

					res.Premio = otorgado.ToString();
				}
				catch( ArgumentOutOfRangeException e )
				{
					string msg = string.Format( "No se pudieron asignaron los premios. Index out of bounds: {0}", i );
					log.LogError( e, msg );
					throw new JoserrasException( msg );
				}
				catch(InvalidOperationException ioe )
				{
					string msg = string.Format( "No se pudieron asignaron los premios. Faltó de registrar la Posición: {0}", i );
					log.LogError( ioe, msg );
					throw new JoserrasException( msg );
				}
				catch(Exception e )
				{
					log.LogError( e, e.Message );
					throw new JoserrasException( "Error desconocido. No se pudo asignar el premio de la posición: " + i, e );
				}
			}
		}

		public Bolsa GetBolsaTorneo(int entradas, int buyin, int ligaFee = 0)
		{
			return new Bolsa( (entradas * buyin) - (entradas * ligaFee) );
		}

	}

}