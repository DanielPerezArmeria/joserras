using System;

namespace TorneosWeb.exception
{
	public class JoserrasException : Exception
	{
		public JoserrasException(Exception e) : base(e.Message, e)
		{
			
		}
	}

}