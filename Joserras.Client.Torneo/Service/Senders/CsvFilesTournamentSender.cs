using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Properties;
using Joserras.Client.Torneo.Utils;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service.Senders
{
	public class CsvFilesTournamentSender : ITournamentSender
	{
		private IHttpService httpService;

		public CsvFilesTournamentSender(IHttpService httpService)
		{
			this.httpService = httpService;
		}

		public string SendTournament(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			string path = torneo.GetPath();

			using (MultipartFormDataContent httpContent = new())
			{
				FillContent( path, httpContent );

				Task<HttpResponseMessage> message = httpService.PostTournamentAsync( Resources.API_POST_UPLOAD_FILES, httpContent );
				if (message.Result.IsSuccessStatusCode)
				{
					return "El torneo fue agregado con éxito.";
				}

				return "No se pudo crear el torneo." + "\n" + message.Result.Content.ReadAsStringAsync();
			}
		}

		public async Task<string> SendTournamentAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			string path = torneo.GetPath();

			using (MultipartFormDataContent httpContent = new())
			{
				FillContent( path, httpContent );

				HttpResponseMessage message = await httpService.PostTournamentAsync( Resources.API_POST_UPLOAD_FILES, httpContent );
				if (message.IsSuccessStatusCode)
				{
					return "El torneo fue agregado con éxito.";
				}

				string errorMsg = await message.Content.ReadAsStringAsync();
				return "No se pudo crear el torneo." + "\n" + errorMsg;
			}
		}

		private void FillContent(string path, MultipartFormDataContent httpContent)
		{
			foreach (string fileName in Directory.GetFiles( path ))
			{
				FileStream stream = File.OpenRead( fileName );
				StreamContent streamContent = new StreamContent( stream );
				streamContent.Headers.Add( "Content-Type", "text/csv" );
				streamContent.Headers.Add( "Content-Disposition",
					"form-data; name=\"files\"; filename=\"" + Path.GetFileName( stream.Name ) + "\"" );
				httpContent.Add( streamContent, "files", Path.GetFileName( stream.Name ) );
			}
		}

	}

}