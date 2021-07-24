using Joserras.Client.Torneo.Model;
using System.Windows;

namespace Joserras.Client.Torneo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public ApplicationModel AppModel;

		public MainWindow(ApplicationModel appModel)
		{
			AppModel = appModel;

			DataContext = appModel;
			InitializeComponent();
		}
	}
}
