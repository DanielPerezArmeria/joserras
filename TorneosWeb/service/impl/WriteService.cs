using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using TorneosWeb.domain.dto;
using TorneosWeb.domain.models;
using TorneosWeb.exception;

namespace TorneosWeb.service.impl
{
	public class WriteService : IWriteService
	{
		private IReadService readService;
		private ICacheService cacheService;
		private ILogger<WriteService> log;

		private string connString;

		public WriteService(IReadService service, ICacheService cacheService, IConfiguration config, ILogger<WriteService> logger)
		{
			readService = service;
			this.cacheService = cacheService;
			log = logger;
			connString = config.GetConnectionString( Properties.Resources.joserrasDb );
		}

		public void uploadTournament(List<IFormFile> files)
		{
			TorneoDTO torneo = GetDTO<TorneoDTO>( "torneo", files );
			List<DetalleTorneoDTO> detalles = GetDTO<List<DetalleTorneoDTO>>( "detalle", files );
			List<EliminacionesDTO> kos = GetDTO<List<EliminacionesDTO>>( "knockouts", files );

			SqlTransaction transaction = null;
			try
			{
				using( SqlConnection connection = new SqlConnection( connString ) )
				{
					connection.Open();
					transaction = connection.BeginTransaction();

					insertarNuevosJugadores( detalles, connection, transaction );

					torneo.Id = insertarTorneo( torneo, detalles, connection, transaction );

					insertarDetallesTorneo( torneo.Id, detalles, connection, transaction );

					if(kos != null && kos.Count > 0 )
					{
						insertarKos( torneo.Id, kos, connection, transaction );
					}

					transaction.Commit();
				}
				cacheService.Clear();
			}
			catch(Exception e )
			{
				log.LogError( e, e.Message );
				try
				{
					transaction.Rollback();
				}
				catch( Exception xe )
				{
					log.LogError( xe, xe.Message );
					throw;
				}
				throw new JoserrasException( e );
			}
		}

		private void insertarKos(Guid torneoId, List<EliminacionesDTO> kos, SqlConnection conn, SqlTransaction tx)
		{
			string query = "insert into eliminaciones (torneo_id, jugador_id, eliminado_id, eliminaciones) values('{0}', "
				+ "(select id from jugadores where nombre = '{1}'), (select id from jugadores where nombre = '{2}'), {3})";

			foreach(EliminacionesDTO dto in kos )
			{
				string q = string.Format( query, torneoId, dto.Jugador, dto.Eliminado, dto.Eliminaciones );
				new SqlCommand( q, conn, tx ).ExecuteNonQuery();
			}
		}

		private void insertarDetallesTorneo(Guid torneoId, List<DetalleTorneoDTO> detalles, SqlConnection conn, SqlTransaction tx)
		{
			string query = "insert into DetalleTorneos values('{0}', (select id from jugadores where nombre='{1}'), "
				+ "{2}, {3}, '{4}', {5}, '{6}')";

			foreach( DetalleTorneoDTO dto in detalles )
			{
				string q = string.Format( query,
					torneoId, dto.Jugador, dto.Rebuys, dto.Posicion, dto.Podio.ToString(), dto.Premio, dto.Burbuja.ToString() );
				new SqlCommand( q, conn, tx ).ExecuteNonQuery();
			}
		}

		private Guid insertarTorneo(TorneoDTO torneo, List<DetalleTorneoDTO> detalles, SqlConnection conn, SqlTransaction tx)
		{
			int rebuys = detalles.Sum( d => d.Rebuys );
			int bolsa = (torneo.PrecioBuyin * detalles.Count) + (torneo.PrecioRebuy * rebuys);
			string query = string.Format( Properties.Queries.InsertTorneo,
				torneo.Fecha.ToString( "yyyy-MM-dd" ), torneo.PrecioBuyin, torneo.PrecioRebuy, detalles.Count, rebuys, bolsa );

			Guid torneoId = Guid.Parse(new SqlCommand( query, conn, tx ).ExecuteScalar().ToString());

			return torneoId;
		}

		private void insertarNuevosJugadores(List<DetalleTorneoDTO> detalles, SqlConnection conn, SqlTransaction tx)
		{
			List<Jugador> jugadores = readService.GetAllJugadores();
			List<DetalleTorneoDTO> newPlayers = new List<DetalleTorneoDTO>();
			string query = "insert into jugadores (nombre) values ('{0}');";
			foreach(DetalleTorneoDTO dto in detalles )
			{
				if(jugadores.Find(d=>d.Nombre == dto.Jugador) == null )
				{
					new SqlCommand( string.Format( query, dto.Jugador ), conn, tx ).ExecuteNonQuery();
				}
			}
		}

		private T GetDTO<T>(string fileName, List<IFormFile> files)
		{
			IFormFile file = files.Find( t => t.FileName.Contains( fileName ) );
			if(file == null )
			{
				return default(T);
			}
			string dtoString = ReadWholeFile( file );
			return JsonConvert.DeserializeObject<T>( dtoString );
		}

		private string ReadWholeFile(IFormFile file)
		{
			StringBuilder result = new StringBuilder();
			using( var reader = new StreamReader( file.OpenReadStream() ) )
			{
				while( reader.Peek() >= 0 )
					result.AppendLine( reader.ReadLine() );
			}
			return result.ToString();
		}

	}

}