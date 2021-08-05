using Joserras.Client.Torneo.Domain;
using System;

namespace Joserras.Client.Torneo.Model
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