﻿using Joserras.Commons.Domain;
using Joserras.Commons.Dto;
using Joserras.Commons.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
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

		public void SetPremiosTorneo(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados)
		{
			IEnumerable<string> premios = torneo.Premiacion.Split( PrizeFill.SEPARATOR );

			int placesAwarded = premios.Count();
			for( int i = placesAwarded; i > 0; i-- )
			{
				try
				{
					ResultadosDTO res = resultados.First( r => r.Posicion == i );
					string premio = premios.ElementAt( i - 1 );

					IPrizeFiller selectedFiller = fillers.SingleOrDefault( f => f.CanHandle( torneo, resultados, torneo.Bolsa, premio ) );
					if( selectedFiller == null )
					{
						throw new JoserrasException( "No se pudo seleccionar un Prize Filler para la posición: " + i );
					}

					res.Premio = selectedFiller.AssignPrize( torneo, resultados, torneo.Bolsa, premio );
				}
				catch( JoserrasException je )
				{
					log.LogError( je, je.Message );
					throw;
				}
				catch( ArgumentOutOfRangeException e )
				{
					string msg = string.Format( "No se pudieron asignaron los premios. Index out of bounds: {0}", i );
					log.LogError( e, msg );
					throw new JoserrasException( msg );
				}
				catch( InvalidOperationException ioe )
				{
					string msg = string.Format( "No se pudieron asignaron los premios. Faltó de registrar la Posición: {0}", i );
					log.LogError( ioe, msg );
					throw new JoserrasException( msg );
				}
				catch( Exception e )
				{
					log.LogError( e, e.Message );
					throw new JoserrasException( "Error desconocido. No se pudo asignar el premio de la posición: " + i, e );
				}
			}
		}

		public string SetPremiacionString(TorneoDTO torneo, IEnumerable<ResultadosDTO> resultados)
		{
			string premiacion = string.Empty;
			int entradas = torneo.Entradas + torneo.Rebuys;
			if( resultados.Any( r => !r.Premio.IsNullEmptyOrZero() ) ) {
				List<string> list = resultados.Where( r => !r.Premio.IsNullEmptyOrZero() )
						.OrderBy( r => r.Posicion ).Select( r => r.Premio ).ToList();
				premiacion = string.Join( PrizeFill.SEPARATOR, list );
			}
			else if( !string.IsNullOrEmpty( torneo.Premiacion ) )
			{
				premiacion = torneo.Premiacion;
			}
			else
			{
				IEnumerable<PrizeRange> PrizeRanges = prizeDao.GetPrizeRanges();
				PrizeRange selectedRange = PrizeRanges.First( r => r.IsBetween( entradas ) );
				log.LogDebug( "Selected prize range: {0}", selectedRange.ToString() );
				premiacion = selectedRange.Premiacion;
			}

			log.LogDebug( "Premiación del torneo es: " + premiacion );
			return premiacion;
		}

		public Bolsa GetBolsaTorneo(int entradas, int rebuys, int buyinPrice, int rebuyPrice)
		{
			return new Bolsa( (entradas * buyinPrice) + (rebuys * rebuyPrice) );
		}

	}

}