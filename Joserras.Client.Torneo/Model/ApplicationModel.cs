﻿using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.Properties;

namespace Joserras.Client.Torneo.Model
{
	public class ApplicationModel : ViewModel
	{

		public ApplicationModel(TorneoViewModel torneoModel, ResultadosViewModel resModel, KnockoutsViewModel koModel)
		{
			TorneoModel = torneoModel;
			KoModel = koModel;
			ResModel = resModel;

			RootDir = Settings.Default.RootDir;
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
			set { SetProperty( ref isReady, value ); }
		}


		public void ChangeRootDir(string newRootDir)
		{
			Settings.Default.RootDir = newRootDir;
			Settings.Default.Save();

			RootDir = Settings.Default.RootDir;
		}

	}

}