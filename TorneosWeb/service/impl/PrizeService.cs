using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.domain.dto;
using TorneosWeb.exception;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class PrizeService : IPrizeService
	{
		private readonly string ConnString;
		private JoserrasQuery joserrasQuery;
		private ILogger<PrizeService> log;

		private List<PrizeRangeDto> PrizeRanges { get; set; }

		public PrizeService(IConfiguration conf, JoserrasQuery joserrasQuery, ILogger<PrizeService> logger)
		{
			ConnString = conf.GetConnectionString( Properties.Resources.joserrasDb );
			this.joserrasQuery = joserrasQuery;
			log = logger;
			PrizeRanges = new List<PrizeRangeDto>();
		}

		public void SetPremiosTorneo(TorneoDTO torneo, List<ResultadosDTO> resultados)
		{
			if(PrizeRanges.Count == 0 )
			{
				FillPrizeRanges();
			}

			int entradas = torneo.Entradas + torneo.Rebuys;
			PrizeRangeDto selectedRange = PrizeRanges.First( r => r.IsBetween( entradas ) );

			decimal bolsa = torneo.Bolsa;

			FillPrizes( torneo, resultados, selectedRange, bolsa );
		}

		private void FillPrizes(TorneoDTO torneo, List<ResultadosDTO> resultados, PrizeRangeDto selectedRange, decimal bolsa)
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
					if( premio.Contains( '%' ) )
					{
						decimal factor = decimal.Parse( premio.Replace( "%", "" ) ) / 100;
						res.Premio = (bolsa * factor).ToString();
					}
					else if( premio.Contains( 'x' ) )
					{
						decimal factor = decimal.Parse( premio.Replace( "x", "" ) );
						res.Premio = (torneo.PrecioBuyin * factor).ToString();
						bolsa -= res.Premio.ToDecimal();
					}
					else if( premio.Contains( 'p' ) )
					{
						decimal factor = decimal.Parse( premio.Replace( "p", "" ) ) / 100;
						res.Premio = (bolsa * factor).ToString();
						bolsa -= res.Premio.ToDecimal();
					}
					else
					{
						res.Premio = premio;
						bolsa -= res.Premio.ToDecimal();
					}
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
					throw new JoserrasException( "Error desconocido", e );
				}
			}
		}

		private void FillPrizeRanges()
		{
			string query = "select * from premiaciones";
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					string range = reader.GetString( 1 );
					string premio = reader.GetString( 2 );
					string[] ranges = range.Split( "-" );
					if(ranges.Length > 1 )
					{
						PrizeRanges.Add( new PrizeRangeDto( int.Parse( ranges[ 0 ] ), int.Parse( ranges[ 1 ] ), premio ) );
					}
					else
					{
						PrizeRanges.Add( new PrizeRangeDto( int.Parse( ranges[ 0 ] ), int.MaxValue, premio ) );
					}
				}
			} );
		}

		public decimal GetBolsaTorneo(int entradas, int buyin, int ligaFee = 0)
		{
			return (entradas * buyin) - (entradas * ligaFee);
		}


		private class PrizeRangeDto : IEquatable<PrizeRangeDto>
		{
			public int Menor { get; set; }
			public int Mayor { get; set; }
			public string Premiacion { get; set; }

			public PrizeRangeDto(int menor, int mayor, string premiacion)
			{
				Menor = menor;
				Mayor = mayor;
				Premiacion = premiacion;
			}

			public bool IsBetween(int x)
			{
				return Menor <= x && x <= Mayor;
			}

			public bool Equals(PrizeRangeDto other)
			{
				return Menor == other.Menor && Mayor == other.Mayor;
			}

		}

	}

}