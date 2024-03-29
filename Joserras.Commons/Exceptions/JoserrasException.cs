﻿using System;

namespace Joserras.Commons.Exceptions
{
	public class JoserrasException : Exception
	{
		public JoserrasException(Exception e) : base(e.Message, e ) { }

		public JoserrasException(string message) : base( message ) { }

		public JoserrasException(string message, Exception e) : base(message, e ) { }

	}

}