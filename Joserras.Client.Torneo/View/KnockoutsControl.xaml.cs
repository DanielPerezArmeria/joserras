using Joserras.Client.Torneo.Model;
using System.Windows;
using System.Windows.Controls;

namespace Joserras.Client.Torneo.View
{
	/// <summary>
	/// Interaction logic for KnockoutsControl.xaml
	/// </summary>
	public partial class KnockoutsControl : UserControl
	{

		public KnockoutsControl()
		{
			InitializeComponent();
		}

		private void delButton_Click(object sender, RoutedEventArgs e)
		{
			KO ko = ((Button)sender).DataContext as KO;
			if (ko == null)
			{
				return;
			}

			(DataContext as KnockoutsViewModel).Kos.Remove( ko );
		}

	}

}