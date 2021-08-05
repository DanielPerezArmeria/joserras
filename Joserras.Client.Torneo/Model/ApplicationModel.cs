using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.Properties;
using Joserras.Client.Torneo.Utils;
using Microsoft.Extensions.Logging;

namespace Joserras.Client.Torneo.Model
{
	public class ApplicationModel : ViewModel
	{

		private ILogger<ApplicationModel> log;

		public ApplicationModel(TorneoViewModel torneoModel, ResultadosViewModel resModel, KnockoutsViewModel koModel,
			ILogger<ApplicationModel> logger)
		{
			TorneoModel = torneoModel;
			KoModel = koModel;
			ResModel = resModel;
			log = logger;

			ResModel.Buyin = TorneoModel.PrecioBuyin;
			TorneoModel.PropertyChanged += ResModel.TorneoModel_PropertyChanged;

			IsEnabled = true;

			RootDir = Settings.Default.RootDir;

			log.LogDebug( "ApplicationMode initialized" );
		}


		private bool isEnabled;
		public bool IsEnabled
		{
			get { return isEnabled; }
			set { SetProperty( ref isEnabled, value ); }
		}


		private string rootDir;
		public string RootDir
		{
			get { return rootDir; }
			set { SetProperty( ref rootDir, value ); }
		}

		private TorneoViewModel torneoModel;
		public TorneoViewModel TorneoModel
		{
			get { return torneoModel; }
			set { SetProperty( ref torneoModel, value ); }
		}

		private KnockoutsViewModel koModel;
		public KnockoutsViewModel KoModel
		{
			get { return koModel; }
			set { SetProperty( ref koModel, value ); }
		}

		private ResultadosViewModel resModel;
		public ResultadosViewModel ResModel
		{
			get { return resModel; }
			set { SetProperty( ref resModel, value ); }
		}

		private bool isReady;
		public bool IsReady
		{
			get { return isReady; }
			set
			{
				SetProperty( ref isReady, value );
				CrearTorneoCommand.RaiseCanExecuteChanged();
			}
		}


		public void ChangeRootDir(string newRootDir)
		{
			Settings.Default.RootDir = newRootDir;
			Settings.Default.Save();

			RootDir = Settings.Default.RootDir;
		}

		public DelegateCommand crearTorneoCommand;
		public DelegateCommand CrearTorneoCommand
		{
			get { return crearTorneoCommand; }
			set { SetProperty( ref crearTorneoCommand, value ); }
		}

	}

}