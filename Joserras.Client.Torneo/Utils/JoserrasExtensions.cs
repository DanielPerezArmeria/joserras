using Joserras.Client.Torneo.Model;
using Joserras.Client.Torneo.Properties;

namespace Joserras.Client.Torneo.Utils
{
	public static class JoserrasExtensions
	{
		public static string GetPath(this TorneoViewModel torneo)
		{
			string rootDir = Settings.Default.RootDir;
			string path = rootDir + @"\" + torneo.Fecha.Year + @"\" + torneo.Fecha.ToString( "MMMM" ) + @"\";
			path += torneo.Fecha.ToString( "yyyy-MM-dd" ) + @"\";

			return path;
		}

	}

}