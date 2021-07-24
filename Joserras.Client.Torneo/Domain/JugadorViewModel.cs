using Joserras.Client.Torneo.Model;
using System;

namespace Joserras.Client.Torneo.Domain
{
	public class JugadorViewModel : ViewModel<Guid>
	{
		private string nombre;

		public string Nombre
		{
			get { return nombre; }
			set { SetProperty( ref nombre, value ); }
		}

	}

}