using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.domain.azure;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.service;
using TorneosWeb.util;
using TorneosWeb.util.PointRules;

namespace TorneosWeb.dao.impl
{
	public class AzureTableStorageDao : IStorageDao
	{
		private readonly ILogger<AzureTableStorageDao> log;
		private readonly IAzureTableFinder tableFinder;

		public AzureTableStorageDao(IAzureTableFinder tableFinder, ILogger<AzureTableStorageDao> logger)
		{
			this.tableFinder = tableFinder;
      log = logger;
		}

		public void SaveLigaStandings(Guid id, List<Standing> standings)
		{
			InsertStandings( id, standings, typeof( PuntosLiga ) );
		}

		public void SaveTorneoStandings(Guid id, List<Standing> standings)
		{
			InsertStandings( id, standings, typeof( PuntosTorneo ) );
		}

		private void InsertStandings(Guid id, List<Standing> standings, Type T)
		{
			CloudTable table = tableFinder.GetTable( T.Name );

			TableBatchOperation batchOperation = new();

			foreach (Standing standing in standings)
			{
				AbstractPuntosAzureEntity puntos = (AbstractPuntosAzureEntity)Activator.CreateInstance( T, new object[] { id.ToString(), standing.JugadorId.ToString() } );
				puntos.Standings = new SortedDictionary<PointRuleType, FDecimal>( standing.Puntos );

				batchOperation.InsertOrReplace( puntos );
			}

			log.LogDebug( "Saving points in table '{0}' for Id '{1}'", T.Name, id );
			table.ExecuteBatch( batchOperation );
			log.LogDebug( "Records inserted" );
		}

	}

}