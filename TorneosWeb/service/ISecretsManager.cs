namespace TorneosWeb.service
{
	public interface ISecretsManager
	{
		string GetSecret(string secretName);
	}

}