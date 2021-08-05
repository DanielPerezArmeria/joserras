using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Properties;
using Joserras.Client.Torneo.Utils;
using Joserras.Commons.Domain;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Joserras.Client.Torneo.Service.Impl
{

	public class JoserrasService : IJoserrasService
	{
		private ApplicationModel AppModel;
		private readonly IHttpService httpService;
		private KnockoutsViewModel KoModel;
		private ResultadosViewModel ResModel;
		private ITournamentCreator creator;

		public JoserrasService(ApplicationModel model, IHttpService service, ITournamentCreator creator)
		{
			AppModel = model;
			KoModel = AppModel.KoModel;
			ResModel = AppModel.ResModel;
			httpService = service;
			this.creator = creator;
		}

		public void Init()
		{
			try
			{
				List<JugadorViewModel> jugadores = httpService.Get<List<JugadorViewModel>>( Resources.API_GET_JUGADORES );
				ResModel.Jugadores = jugadores;
				KoModel.Jugadores = jugadores;

				ResModel.PrizeRanges = httpService.Get<IEnumerable<PrizeRange>>( Resources.API_GET_PRIZE_RANGES );

				AppModel.CrearTorneoCommand = new DelegateCommand( CrearTorneo, CanCrearTorneo );
			}
			catch (Exception e)
			{
				MessageBox.Show( e.Message );
			}
		}

		private bool CanCrearTorneo(object arg)
		{
			return AppModel.IsReady;
		}

		private async void CrearTorneo()
		{
			AppModel.IsEnabled = false;

			await creator.CreateTournamentAsync( AppModel.TorneoModel, ResModel.AsList(), KoModel.AsList() );

			AppModel.IsEnabled = true;
			AppModel.IsReady = false;
		}

	}

}