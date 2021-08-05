using Joserras.Client.Torneo.Service;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System.Windows;

namespace Joserras.Client.Torneo
{
	public class Startup
	{
		private Container container;
		private ILogger<Startup> log;

		public Startup(Container container, ILogger<Startup> logger)
		{
			this.container = container;
			log = logger;
		}

		public void Run()
		{
			log.LogDebug( "App starting up !" );

			container.GetInstance<IJoserrasService>().Init();

			MainWindow window = container.GetInstance<MainWindow>();
			Application.Current.MainWindow = window;
			window.Show();
		}

	}

}