using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
		private ITorneoDataReader ligaDataReader;
		private ILigaReader ligaReader;
		private IConfiguration config;

		public LigaWriter(IReadService readService, ITorneoDataReader dataReader, ILigaReader ligaReader, IConfiguration config)
		{
			this.readService = readService;
			ligaDataReader = dataReader;
			this.ligaReader = ligaReader;
		}

		public void InsertarTorneoDeLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos )
		{
			TorneoUnitOfWork uow = null;

			using( uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				/*Liga liga = ligaReader.FindLigaByNombre( torneo.Liga );
				if( liga == null )
				{
					LigaDTO ligaDto = new LigaDTO();
					ligaDto.Nombre = torneo.Liga;
					ligaDto.Abierta = true;
					ligaDto.Id = InsertarNuevaLiga( ligaDto, uow );
				}

				InsertarTorneoDeLiga( liga, torneo, uow ); */
			}
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
			uow.ExecuteNonQuery( query, liga.Id, torneo.Id );
		}

		private void InsertarPuntaje(Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			string[] reglas = liga.Puntaje.Split( ";" );
			foreach(string pRule in reglas )
			{

			}
		}

		public void InsertarTorneoDeLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos, TorneoUnitOfWork uow)
		{
			throw new NotImplementedException();
		}

		public void AgregarNuevaLiga(IFormFile file)
		{
			LigaDTO liga = ligaDataReader.GetItems<LigaDTO>( file ).First();
			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				string query = @"insert into ligas (nombre, abierta, puntaje) output values ('{0}', {1}, '{2}')";
				uow.ExecuteNonQuery( query, liga.Nombre, 1, liga.Puntaje );
			}
		}

	}

}