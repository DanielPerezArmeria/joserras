using AutoMapper;
using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Properties;
using Joserras.Commons.Dto;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service.Senders
{
	public class JsonTournamentSender : ITournamentSender
	{
		private IHttpService httpService;
		private IMapper mapper;

		public JsonTournamentSender(IHttpService httpService, IMapper mapper)
		{
			this.httpService = httpService;
			this.mapper = mapper;
		}

		public string SendTournament(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			TorneoUploadWrapper wrapper = new TorneoUploadWrapper( mapper.Map<TorneoDTO>(torneo),
				mapper.Map<List<ResultadosDTO>>( resultados ), mapper.Map<List<KnockoutsDTO>>( kos ) );

			HttpContent content = CreateContent( wrapper );

			Task<HttpResponseMessage> message = httpService.PostTournamentAsync( Resources.API_POST_UPLOAD_JSON, content );
			if (message.Result.IsSuccessStatusCode)
			{
				return "El torneo fue agregado con éxito.";
			}

			return "No se pudo crear el torneo." + "\n" + message.Result.Content.ReadAsStringAsync();
		}

		public async Task<string> SendTournamentAsync(TorneoViewModel torneo, List<Resultado> resultados, List<KO> kos)
		{
			TorneoUploadWrapper wrapper = new TorneoUploadWrapper( mapper.Map<TorneoDTO>( torneo ),
				mapper.Map<List<ResultadosDTO>>( resultados ), mapper.Map<List<KnockoutsDTO>>( kos ) );

			HttpContent content = CreateContent( wrapper );

			HttpResponseMessage message = await httpService.PostTournamentAsync( Resources.API_POST_UPLOAD_JSON, content );
			if (message.IsSuccessStatusCode)
			{
				return "El torneo fue agregado con éxito.";
			}

			return "No se pudo crear el torneo." + "\n" + message.Content.ReadAsStringAsync().Result;
		}

		private HttpContent CreateContent(TorneoUploadWrapper wrapper)
		{
			MediaTypeHeaderValue header = new( "application/json" );
			return JsonContent.Create( wrapper, header );
		}

	}

}