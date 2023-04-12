using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using TorneosWeb.config;

namespace TorneosWeb.service.impl
{
	public class SecretsManager : ISecretsManager
	{
		private readonly Dictionary<string, string> cache;
		private SecretClient secretClient;
		private ILogger<SecretsManager> log;

		public SecretsManager(SecretsConfig config, ILogger<SecretsManager> logger)
		{
			cache = new();
			log = logger;

			SecretClientOptions options = new()
			{
				Retry =
				{
						Delay= TimeSpan.FromSeconds(2),
						MaxDelay = TimeSpan.FromSeconds(16),
						MaxRetries = 3,
						Mode = RetryMode.Exponential
				 }
			};

			secretClient = new SecretClient( new Uri( config.SecretsUri ), new DefaultAzureCredential(), options );
			foreach(string secret in config.Secrets )
			{
				try
				{
					KeyVaultSecret keyVaultSecret = secretClient.GetSecret( secret );
					string key = string.Format( "{0}:{1}", nameof( GetSecret ), secret );
					cache.Add( key, keyVaultSecret.Value );
					log.LogDebug( "Secret '{0}' added", secret );
				}
				catch( Exception e )
				{
					log.LogWarning( e, "Could not get secret '{0}'", secret );
				}
			}
		}

		public string GetSecret(string secretName)
		{
			string key = string.Format( "{0}:{1}", nameof( GetSecret ), secretName );
			if( cache.ContainsKey( key ) )
			{
				return cache[key];
			}

			log.LogWarning( "Secret '{0}' could not be found in Manager" );
			return null;
		}

	}

}