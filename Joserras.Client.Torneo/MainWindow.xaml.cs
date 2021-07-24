using Joserras.Client.Torneo.Model;
using System.Windows;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

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

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.SelectedPath = AppModel.RootDir;

			dialog.ShowDialog( this );

			AppModel.ChangeRootDir( dialog.SelectedPath );
		}

		private void createButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}

}