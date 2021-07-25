using System.Net.Http;
using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service
{
	public interface IHttpService
	{
		T Get<T>(string apiCall);

		Task<T> GetAsync<T>(string apiCall);

		HttpResponseMessage PostTournament(string apiCall, HttpContent content);

		Task<HttpResponseMessage> PostTournamentAsync(string apiCall, HttpContent content);
	}
}