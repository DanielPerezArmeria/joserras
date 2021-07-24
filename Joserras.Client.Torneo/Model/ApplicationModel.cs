using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.View;

namespace Joserras.Client.Torneo.Model
{
	public class ApplicationModel :ViewModel
	{

		public ApplicationModel(TorneoViewModel torneoModel, ResultadosViewModel resModel, KnockoutsControl koControl)
		{
			TorneoModel = torneoModel;
			KoControl = koControl;
			ResModel = resModel;
		}

		
		private TorneoViewModel torneoModel;
		public TorneoViewModel TorneoModel
		{
			get { return torneoModel; }
			set { SetProperty( ref torneoModel, value ); }
		}

		private KnockoutsControl koControl;
		public KnockoutsControl KoControl
		{
			get { return koControl; }
			set { SetProperty( ref koControl, value ); }
		}

		private ResultadosViewModel resModel;
		public ResultadosViewModel ResModel
		{
			get { return resModel; }
			set { SetProperty( ref resModel, value ); }
		}

	}

}