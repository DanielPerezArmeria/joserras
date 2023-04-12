using System.Collections.Generic;

namespace TorneosWeb.config
{
	public class SecretsConfig
	{
		public string SecretsUri { get; set; }
		public IList<string> Secrets { get; set; }
	}

}