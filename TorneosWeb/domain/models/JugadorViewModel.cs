using System;
using System.Collections.Generic;
using static TorneosWeb.Pages.JugadorModel;

namespace TorneosWeb.domain.models
{
	public class JugadorViewModel
	{
		public DetalleJugador DetalleJugador { get; set; }
		public List<JugadorTorneosViewModel> Torneos { get; set; }
		public IDictionary<int, int> Podios { get; set; }
		public string DataPoints { get; set; }
	}

}