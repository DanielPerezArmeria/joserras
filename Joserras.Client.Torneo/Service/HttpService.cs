using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service
{
	public class HttpService
	{
		private readonly Uri BaseAddress;

		public HttpService(string baseAddress)
		{
			BaseAddress = new Uri( baseAddress );
		}

		public async Task<T> GetAsync<T>(string apiCall)
		{
			using( HttpClient client = new() )
			{
				client.BaseAddress = BaseAddress;
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

				HttpResponseMessage response = new HttpResponseMessage();
				response = await client.GetAsync( apiCall );
				response.EnsureSuccessStatusCode();
				string body = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<T>( body );
			}
		}

	}

}