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

		public NullAzureDaoDecorator(IStandingsDao<T> wrapped, IOptions<AzureTableConfig> azureTableConfig)
		{
			this.wrapped = wrapped;
			IsAzureEnabled = azureTableConfig.Value.Enabled;
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