using System.Threading.Tasks;

namespace Joserras.Client.Torneo.Service
{
	public interface IHttpService
	{
		Task<T> GetAsync<T>(string apiCall);
	}
}