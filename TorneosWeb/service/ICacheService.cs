namespace TorneosWeb.service
{
	public interface ICacheService
	{
		void Clear();

		void Add(string key, object value);

		T Get<T>(string key);

		bool ContainsKey(string key);
	}

}