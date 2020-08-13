using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
		private ILogger<LigaWriter> log;

		private IReadService readService;
		private ITorneoDataReader ligaDataReader;
		private ILigaReader ligaReader;
		private IConfiguration config;
		private ICacheService cacheService;

		public LigaWriter(IReadService readService, ITorneoDataReader dataReader, ILigaReader ligaReader,
				IConfiguration config, ICacheService cacheService, ILogger<LigaWriter> log)
		{
			this.readService = readService;
			ligaDataReader = dataReader;
			this.ligaReader = ligaReader;
			this.config = config;
			this.log = log;
			this.cacheService = cacheService;
		}

		public void InsertarTorneoDeLiga(TorneoDTO torneo, List<ResultadosDTO> resultados, List<KnockoutsDTO> kos)
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
			string query = @"insert into ligas (nombre, abierta, puntaje, fee) output INSERTED.ID values ('{0}', {1}, '{2}', {3})";
			Guid ligaId = Guid.Parse( uow.ExecuteScalar( query, liga.Nombre, liga.Abierta, liga.Puntaje, liga.Fee ).ToString() );
			return ligaId;
		}

		private void InsertarTorneoDeLiga(Liga liga, TorneoDTO torneo, TorneoUnitOfWork uow)
		{
			string query = "insert into torneos_liga values ('{0}', '{1}')";
			uow.ExecuteNonQuery( query, liga.Id, torneo.Id );
		}

		private void InsertarPuntaje(Liga liga, TorneoDTO torneo, List<ResultadosDTO> resultados, TorneoUnitOfWork uow)
		{
			string[] reglas = liga.Puntaje.Split( ";" );
			foreach( string pRule in reglas )
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
				string query = @"insert into ligas (nombre, abierta, puntaje, fee) values ('{0}', {1}, '{2}', {3})";
				uow.ExecuteNonQuery( query, liga.Nombre, 1, liga.Puntaje, liga.Fee );

				uow.Commit();
			}
		}

		public int AsociarTorneoEnFecha(DateTime date)
		{
			int rowsAffected = 0;
			Torneo torneo = readService.FindTorneoByFecha( date );
			if(torneo == null )
			{
				return rowsAffected;
			}
			using(TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				Liga liga = ligaReader.GetCurrentLiga();
				string query = "insert into torneos_liga values ('{0}', '{1}')";
				rowsAffected = uow.ExecuteNonQuery( query, liga.Id, torneo.Id );

				Resultados resultados = readService.FindResultadosTorneo( torneo.Id );
				foreach(Posicion pos in resultados.Posiciones )
				{
					int puntos = liga.PointRules.Sum( p => p.Value.GetPuntos( pos.JugadorId, liga, resultados ) );
					query = "insert into puntos_torneo_liga values ('{0}', '{1}', {2})";
					uow.ExecuteNonQuery( query, torneo.Id, pos.JugadorId, puntos );
				}

				uow.Commit();
			}

			cacheService.Clear();
			return rowsAffected;
		}

	}

}