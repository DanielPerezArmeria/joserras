using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.service
{
	public interface ILigaReader
	{
		Liga FindLigaByNombre(string nombre);

		Liga GetCurrentLiga();
	}

}