using System;
using System.Collections.Generic;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service.impl
{
	public class ChartService : IChartService
	{
		private IReadService readService;
		private ILigaReader ligaReader;
		private ILigaDao ligaDao;
		private IJugadorService jugadorService;

		public ChartService(IReadService readService, ILigaReader ligaReader, ILigaDao ligaDao, IJugadorService jugadorService)
		{
			this.readService = readService;
			this.ligaReader = ligaReader;
			this.ligaDao = ligaDao;
			this.jugadorService = jugadorService;
		}

		public List<ChartDataPoint> GetPlayerProfitChartData(Guid playerId)
		{
			List<Posicion> posiciones = jugadorService.GetAllPosicionesByJugador( playerId );
			List<Torneo> torneos = posiciones.Select( p => p.Torneo ).OrderBy( t => t.Fecha ).ToList();

			List<ChartDataPoint> dataPoints = new();
			Dictionary<DateTime, decimal> pointsDic = new();
			List<Liga> ligas = ligaReader.GetAllLigas();

			foreach (Torneo torneo in torneos)
			{
				Posicion posicion = posiciones.Single( p => p.TorneoId.Equals( torneo.Id ) );
				if (!pointsDic.ContainsKey( torneo.FechaDate ))
				{
					pointsDic.Add( torneo.FechaDate, posicion.ProfitTotalNumber );
				}
				else
				{
					pointsDic[torneo.FechaDate] = pointsDic[torneo.FechaDate] + posicion.ProfitTotalNumber;
				}
			}

			foreach (Liga liga in ligas)
			{
				IEnumerable<LigaProfitsObject> profitObjects = ligaDao.GetLigaProfitsByLiga( liga );
				LigaProfitsObject po = profitObjects.SingleOrDefault( p => p.JugadorId.Equals( playerId ) );
				if (po != null && po.Premios > 0)
				{
					if (pointsDic.ContainsKey( liga.FechaCierreDate.Value ))
					{
						pointsDic[liga.FechaCierreDate.Value] = pointsDic[liga.FechaCierreDate.Value] + po.Premios;
					}
					else
					{
						pointsDic.Add( liga.FechaCierreDate.Value, po.Premios );
					}
				}
			}

			dataPoints =
				pointsDic.OrderBy( p => p.Key ).Select( p => new ChartDataPoint( p.Key.ToShortDateString(), p.Value ) ).ToList();

			decimal? profit = 0;
			foreach(ChartDataPoint point in dataPoints)
			{
				point.Profit += profit;
				profit = point.Profit;
			}

			return dataPoints;
		}

	}

}