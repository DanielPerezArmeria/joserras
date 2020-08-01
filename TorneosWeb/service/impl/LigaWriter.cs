using System;
using System.Collections.Generic;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.dto.ligas;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class LigaWriter : ILigaWriter
	{
		private IReadService readService;
		private JoserrasQuery joserrasQuery;
		private ILigaReader ligaReader;

		public LigaWriter(IReadService readService, JoserrasQuery joserrasQuery, ILigaReader ligaReader)
		{
			this.readService = readService;
			this.joserrasQuery = joserrasQuery;
			this.ligaReader = ligaReader;
		}

		public void InsertarLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow)
		{
			Liga liga = ligaReader.FindLigaByNombre( torneo.Liga );
			if(liga == null )
			{
				LigaDTO ligaDto = new LigaDTO();
				ligaDto.Nombre = torneo.Liga;
				ligaDto.Nombre = torneo.LigaPuntaje;
				ligaDto.Abierta = true;
				ligaDto.Id = InsertarNuevaLiga( ligaDto, uow );
			}

			InsertarTorneoDeLiga( liga, torneo, uow );
		}

		private Guid InsertarNuevaLiga(LigaDTO liga, TorneoUnitOfWork uow)
		{
			string query = @"insert into ligas (nombre, abierta, puntaje) output INSERTED.ID values ('{0}', {1}, '{2}')";
			Guid ligaId = Guid.Parse( uow.ExecuteScalar( query, liga.Nombre, liga.Abierta, liga.Puntaje ).ToString() );
			return ligaId;
		}

		private void InsertarTorneoDeLiga(Liga liga, TorneoDTO torneo, TorneoUnitOfWork uow)
		{
			string query = "insert into torneos_liga values ('{0}', '{1}', {2})";
			uow.ExecuteNonQuery( query, liga.Id, torneo.Id, torneo.LigaFee );
		}

		private void InsertarPuntaje(Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			string[] reglas = liga.Puntaje.Split( ";" );
			foreach(string pRule in reglas )
			{

			}
		}

	}

}