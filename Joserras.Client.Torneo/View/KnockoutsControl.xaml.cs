using Joserras.Client.Torneo.Domain;
using System.Windows.Controls;

namespace Joserras.Client.Torneo.View
{
	/// <summary>
	/// Interaction logic for KnockoutsControl.xaml
	/// </summary>
	public partial class KnockoutsControl : UserControl
	{

		public KnockoutsControl(KnockoutsViewModel model)
		{
			DataContext = model;
			InitializeComponent();
		}

	}

}