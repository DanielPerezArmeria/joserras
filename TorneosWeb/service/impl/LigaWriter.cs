using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
		private IFileService ligaDataReader;
		private ILigaReader ligaReader;
		private IConfiguration config;
		private ICacheService cacheService;

		public LigaWriter(IReadService readService, IFileService dataReader, ILigaReader ligaReader,
				IConfiguration config, ICacheService cacheService, ILogger<LigaWriter> log)
		{
			this.readService = readService;
			ligaDataReader = dataReader;
			this.ligaReader = ligaReader;
			this.config = config;
			this.log = log;
			this.cacheService = cacheService;
		}

		public void AgregarNuevaLiga(IFormFile file)
		{
			LigaDTO liga = ligaDataReader.GetFormFileItems<LigaDTO>( file ).First();
			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				string query = @"insert into ligas (nombre, abierta, puntaje, fee, premiacion) values ('{0}', {1}, '{2}', {3}, '{4}')";
				uow.ExecuteNonQuery( query, liga.Nombre, 1, liga.Puntaje, liga.Fee, liga.Premiacion );

				uow.Commit();
			}

			cacheService.Clear();
		}

		public int AsociarTorneo(Guid torneoId, TorneoUnitOfWork uow)
		{
			int rowsAffected = 0;

			if( ligaReader.GetCurrentLiga() == null )
			{
				log.LogWarning( "No hay una Liga abierta. No se puede asociar el torneo." );
				return rowsAffected;
			}

			Liga liga = ligaReader.GetCurrentLiga();
			string query = "insert into torneos_liga values ('{0}', '{1}')";
			rowsAffected = uow.ExecuteNonQuery( query, liga.Id, torneoId );

			Resultados resultados = readService.FindResultadosTorneo( torneoId );
			foreach( Posicion pos in resultados.Posiciones )
			{
				int puntos = liga.PointRules.Sum( p => p.Value.GetPuntaje( pos.JugadorId, liga, resultados ) );
				query = "insert into puntos_torneo_liga values ('{0}', '{1}', {2})";
				uow.ExecuteNonQuery( query, torneoId, pos.JugadorId, puntos );
			}

			return rowsAffected;
		}

		public int AsociarTorneoEnFecha(DateTime date)
		{
			Torneo torneo = readService.FindTorneoByFecha( date );
			if(torneo == null )
			{
				return 0;
			}

			using( TorneoUnitOfWork uow = new TorneoUnitOfWork( config.GetConnectionString( Properties.Resources.joserrasDb ) ) )
			{
				return AsociarTorneo( torneo.Id, uow );
			}

		}

	}

}