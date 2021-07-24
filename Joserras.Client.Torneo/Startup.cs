using Joserras.Client.Torneo.Service.Impl;
using SimpleInjector;
using System.Windows;

namespace Joserras.Client.Torneo
{
	public class Startup
	{
		private Container container;

		public Startup(Container container)
		{
			this.container = container;
		}

		public void Run()
		{
			container.GetInstance<JoserrasService>().Init();

			MainWindow window = container.GetInstance<MainWindow>();
			Application.Current.MainWindow = window;
			window.Show();
		}

	}

}