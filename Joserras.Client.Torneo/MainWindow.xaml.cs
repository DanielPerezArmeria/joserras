using Joserras.Client.Torneo.Model;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
			AppModel.PropertyChanged += AppModel_PropertyChanged;
			InitializeComponent();
		}

		private void AppModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals( nameof( ApplicationModel.IsEnabled ) ))
			{
				if (AppModel.IsEnabled)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
				else
				{
					Mouse.OverrideCursor = Cursors.Wait;
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
			dialog.SelectedPath = AppModel.RootDir;

			dialog.ShowDialog( this );

			AppModel.ChangeRootDir( dialog.SelectedPath );
		}

	}

}