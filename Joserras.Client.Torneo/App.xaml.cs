using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Service;
using Joserras.Client.Torneo.View;
using SimpleInjector;
using System.Windows;

namespace Joserras.Client.Torneo
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private Container container;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			container = new();

			container.RegisterSingleton<ApplicationModel>();
			container.RegisterSingleton<TorneoViewModel>();
			container.RegisterSingleton<KnockoutsViewModel>();
			container.RegisterSingleton<KnockoutsControl>();
			container.RegisterSingleton<ResultadosViewModel>();

			container.RegisterSingleton<JoserrasService>();
			container.RegisterSingleton( () => new HttpService( @"https://localhost:5001/" ) );

			container.RegisterSingleton<Startup>();
			container.Register<MainWindow>();

			container.Verify();

			container.GetInstance<Startup>().Run();
		}

	}

}