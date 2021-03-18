using System;

namespace TorneosWeb.domain.models
{
	public class LigaProfitsObject
	{
		public Guid JugadorId { get; set; }
		public decimal Premios { get; set; }
		public int Fees { get; set; }
	}

}