using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace Joserras.Client.Torneo.Service.Impl
{
	public class HttpService : IHttpService
	{
		private readonly Uri BaseAddress;
		private ILogger<HttpService> log;

		public HttpService(string baseAddress, ILogger<HttpService> logger)
		{
			BaseAddress = new Uri( baseAddress );
			log = logger;
		}

		public T Get<T>(string apiCall)
		{
			using (HttpClient client = new())
			{
				client.BaseAddress = BaseAddress;
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

				Task<HttpResponseMessage> response = client.GetAsync( apiCall );
				response.Result.EnsureSuccessStatusCode();
				Task<string> body = response.Result.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<T>( body.Result );
			}
		}

		public async Task<T> GetAsync<T>(string apiCall)
		{
			try
			{
				using (HttpClient client = new())
				{
					client.BaseAddress = BaseAddress;
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

					HttpResponseMessage response = await client.GetAsync( apiCall );
					response.EnsureSuccessStatusCode();
					string body = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<T>( body );
				}
			}
			catch (Exception e)
			{
				log.LogError( e.Message, e );
				MessageBox.Show( "Ocurrió un error en la comunicación con el Servidor." );
				Application.Current.Shutdown();
				return default(T);
			}
		}

		public HttpResponseMessage PostTournament(string apiCall, HttpContent content)
		{
			using (HttpClient client = new())
			{
				client.BaseAddress = BaseAddress;
				Task<HttpResponseMessage> response = client.PostAsync( apiCall, content );
				return response.Result;
			}
		}

		public async Task<HttpResponseMessage> PostTournamentAsync(string apiCall, HttpContent content)
		{
			using (HttpClient client = new())
			{
				client.BaseAddress = BaseAddress;
				return await client.PostAsync( apiCall, content );
			}
		}

	}

}