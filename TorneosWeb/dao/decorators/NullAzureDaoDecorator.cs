using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TorneosWeb.config;
using TorneosWeb.domain.azure;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.dao.decorators
{
	public class NullAzureDaoDecorator<T> : IStandingsDao<T> where T : AbstractPuntosAzureEntity
	{
		private readonly IStandingsDao<T> wrapped;
		private readonly bool IsAzureEnabled;
		private readonly ILogger<NullAzureDaoDecorator<T>> log;

		public NullAzureDaoDecorator(IStandingsDao<T> wrapped, IOptions<AzureTableConfig> azureTableConfig, ILogger<NullAzureDaoDecorator<T>> logger)
		{
			this.wrapped = wrapped;
			IsAzureEnabled = azureTableConfig.Value.Enabled;
			log = logger;

			log.LogDebug( "Is Azure enabled? {0}", IsAzureEnabled );
		}

		public void Save(Guid id, List<Standing> standings)
		{
			if (IsAzureEnabled)
			{
				wrapped.Save( id, standings );
			}
		}

	}

}