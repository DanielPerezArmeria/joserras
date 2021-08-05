using Joserras.Client.Torneo.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

		private void DataGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				return;
			}

			DataGrid dg = (sender as DataGrid);
			if (dg.CurrentColumn is DataGridComboBoxColumn)
			{
				dg.BeginEdit();
			}
		}

	}

}