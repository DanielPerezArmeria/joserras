using Microsoft.AspNetCore.Http;
using System;
using TorneosWeb.util;

namespace TorneosWeb.service
{
	public interface ILigaWriter
	{
		void AgregarNuevaLiga(IFormFile file);

		int AsociarTorneoEnFecha(DateTime date);

		int AsociarTorneo(Guid torneoId);

		void CerrarLiga();
	}

}