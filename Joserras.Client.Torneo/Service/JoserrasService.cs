using Joserras.Client.Torneo.Domain;
using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Properties;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Joserras.Client.Torneo.Service
{

	public class JoserrasService
	{
		private ApplicationModel Model;
		private readonly HttpService httpService;
		private KnockoutsViewModel KoModel;
		private ResultadosViewModel ResModel;

		public JoserrasService(ApplicationModel model, KnockoutsViewModel koModel, ResultadosViewModel resModel,
				HttpService service)
		{
			Model = model;
			KoModel = koModel;
			ResModel = resModel;
			httpService = service;
		}

		public async void Init()
		{
			try
			{
				List<JugadorViewModel> jugadores = await httpService.GetAsync<List<JugadorViewModel>>( Resources.API_GET_JUGADORES );
				ResModel.Jugadores = jugadores;
				KoModel.Jugadores = jugadores;
			}
			catch(Exception e)
			{
				MessageBox.Show( e.Message );
			}
		}

	}

}