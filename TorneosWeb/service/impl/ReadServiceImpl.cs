using AutoMapper;
using Joserras.Commons.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TorneosWeb.dao;
using TorneosWeb.domain.models;
using TorneosWeb.domain.models.ligas;
using TorneosWeb.Properties;
using TorneosWeb.util;

namespace TorneosWeb.service.impl
{
	public class ReadServiceImpl : IReadService
	{
		private IMapper mapper;
		private JoserrasQuery joserrasQuery;
		private ILigaDao ligaDao;
		private ITournamentDao tournamentDao;

		public ReadServiceImpl(IMapper mapper, JoserrasQuery joserrasQuery, ILigaDao ligaDao, ITournamentDao tournamentDao)
		{
			this.mapper = mapper;
			this.joserrasQuery = joserrasQuery;
			this.ligaDao = ligaDao;
			this.tournamentDao = tournamentDao;
		}


		#region GetAllKnockouts
		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts()
		{
			return GetAllKnockouts( string.Empty );
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(DateTime start, DateTime end)
		{
			return GetAllKnockouts( QueryUtils.FormatTorneoBetween( start, end ) );
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(Liga liga)
		{
			return GetAllKnockouts( QueryUtils.FormatTorneoIdIn( liga ) );
		}

		private SortedList<string, Dictionary<string, Knockouts>> GetAllKnockouts(string q)
		{
			SortedList<string, Dictionary<string, Knockouts>> knockouts = new SortedList<string, Dictionary<string, Knockouts>>();
			string query = string.Format( Properties.Queries.GetAllKOs, q );

			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Knockouts ko = mapper.Map<SqlDataReader, Knockouts>( reader );
					if( !knockouts.ContainsKey( ko.Nombre ) )
					{
						knockouts.Add( ko.Nombre, new Dictionary<string, Knockouts>() );
					}
					knockouts[ ko.Nombre ].Add( ko.Eliminado, ko );
				}
			} );

			return knockouts;
		}
		#endregion


		#region Get Torneos

		public List<Torneo> GetAllTorneos()
		{
			List<Torneo> torneos = tournamentDao.GetAllTorneos();
			IDictionary<Guid, List<Guid>> torneosLiga = ligaDao.GetTorneosInLigas();

			Dictionary<Guid, Liga> ligas = new();
			foreach(Guid ligaId in torneosLiga.Keys)
			{
				ligas.Add( ligaId, ligaDao.GetLigaById( ligaId ) );
				List<Guid> torneosInLiga = torneosLiga[ligaId];
				torneos.Where( t => torneosInLiga.Contains( t.Id ) ).ToList().ForEach( t => t.Liga = ligas[ligaId] );
			}

			//FillTorneos( torneos );

			return torneos;
		}

		private void FillTorneos(List<Torneo> torneos)
		{
			foreach( Torneo t in torneos )
			{
				// Obtener Resultados
				t.Resultados = FindResultadosTorneo( t.Id );
			}
		}

		public Torneo FindTorneoByFecha(DateTime fecha)
		{
			Torneo torneo = tournamentDao.FindTorneoByFecha( fecha );
			return torneo;
		}

		#endregion


		public Resultados FindResultadosTorneo(Guid torneoId)
		{
			Resultados resultados = new Resultados();
			resultados.Jugadores = new SortedSet<string>();

			Torneo torneo = tournamentDao.GetTorneo( torneoId );
			torneo.Liga = ligaDao.GetLigaByTorneoId( torneo.Id );

			resultados.Torneo = torneo;

			List<Posicion> posiciones = tournamentDao.GetPosicionesByTorneo( torneoId );
			foreach(Posicion pos in posiciones)
			{
				pos.Resultados = resultados;
				pos.Torneo = torneo;
				resultados.Jugadores.Add( pos.Nombre );
			}

			resultados.Posiciones = posiciones;
			
			resultados.Knockouts = GetKnockoutsByTournamentId( torneoId );

			if(resultados.Knockouts.Count > 0)
			{
				resultados.KnockoutList = GetTournamentKOList( torneoId );
			}

			return resultados;
		}

		public DetalleJugador FindDetalleJugador(Guid playerId)
		{
			DetalleJugador detalle = null;

			string query = string.Format( Properties.Queries.GetDetalleJugador, playerId );
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				if( reader.HasRows )
				{
					reader.Read();
					detalle = mapper.Map<DetalleJugador>( reader );
				}
			} );

			// Si el Detalle es nulo, es que el jugador no ha jugado ningún torneo
			if(detalle == null )
			{
				detalle = new DetalleJugador();
			}

			query = string.Format( Queries.GetBundy, "and j.id = '" + playerId + "' " );
			joserrasQuery.ExecuteQuery( query, reader =>
				{
					if (reader.HasRows)
					{
						reader.Read();
						detalle.UltimoLugar = reader.GetInt32( 1 );
					}
				} );

			query = string.Format( Queries.GetPodiosNegativos, "and j.id = '" + playerId + "' " );
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				if (reader.HasRows)
				{
					reader.Read();
					detalle.PodiosNegativos = reader.GetInt32( 1 );
				}
			} );

			//Encontrar profits de ligas
			LigaProfitsObject ligaProfits = ligaDao.GetTotalLigaProfitsByPlayerId( playerId );
			detalle.PremiosLigaNumber = ligaProfits.Premios;
			detalle.CostosLigaNumber = ligaProfits.Fees;

			detalle.Knockouts = GetKnockoutsByPlayer( playerId );

			return detalle;
		}

		public DetalleJugador FindDetalleJugador(string nombre)
		{
			throw new NotImplementedException();
		}

		public SortedList<string, Dictionary<string, Knockouts>> GetKnockoutsByTournamentId(Guid torneoId)
		{
			SortedList<string, Dictionary<string, Knockouts>> knockouts = new();
			string query = string.Format( "select j.nombre, elim.nombre as eliminado, sum(e.eliminaciones) as eliminaciones, '' as mano from knockouts e, jugadores j, jugadores elim"
					+ " where torneo_id = '{0}' and e.jugador_id = j.id and e.eliminado_id = elim.id group by j.nombre, elim.nombre", torneoId );

			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					Knockouts ko = mapper.Map<SqlDataReader, Knockouts>( reader );
					if( !knockouts.ContainsKey( ko.Nombre ) )
					{
						knockouts.Add( ko.Nombre, new Dictionary<string, Knockouts>() );
					}
					knockouts[ ko.Nombre ].Add( ko.Eliminado, ko );
				}
			} );

			return knockouts;
		}

		public List<Knockouts> GetKnockoutsByPlayer(Guid playerId)
		{
			List<Knockouts> kos = new List<Knockouts>();
			string query = string.Format( Properties.Queries.GetKnockoutsByPlayer, playerId );

			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					kos.Add( mapper.Map<SqlDataReader, Knockouts>( reader ) );
				}
			} );

			return kos;
		}

		public List<Knockouts> GetTournamentKOList(Guid torneoId)
		{
			List<Knockouts> kos = new List<Knockouts>();
			string query = string.Format( "select j.nombre, elim.nombre as eliminado, 0.0 as eliminaciones, k.mano_url as mano from knockouts k, jugadores j, jugadores elim, torneos t"
				+ " where k.jugador_id = j.id and k.eliminado_id = elim.id and k.torneo_id = t.id and t.id = '{0}' and k.mano_url is not null", torneoId );

			joserrasQuery.ExecuteQuery( query, reader =>
			 {
				  while (reader.Read())
				 {
					 kos.Add( mapper.Map<SqlDataReader, Knockouts>( reader ) );
				 }
			 } );

			return kos;
		}


		#region GetAllDetalleJugador

		public List<DetalleJugador> GetAllDetalleJugador()
		{
			List<DetalleJugador> detalles = GetAllDetalleJugador( string.Empty );

			foreach(LigaProfitsObject profitObject in ligaDao.GetTotalLigaProfits() )
			{
				DetalleJugador det = detalles.Single( d => d.Id == profitObject.JugadorId );
				det.CostosLigaNumber = profitObject.Fees;
				det.PremiosLigaNumber = profitObject.Premios;
			}

			return detalles;
		}

		public List<DetalleJugador> GetAllDetalleJugador(DateTime start, DateTime end)
		{
			List<DetalleJugador> detalles = GetAllDetalleJugador( QueryUtils.FormatTorneoBetween( start, end ) );

			return detalles;
		}

		public List<DetalleJugador> GetAllDetalleJugador(Liga liga)
		{
			List<DetalleJugador> detalles = GetAllDetalleJugador( QueryUtils.FormatTorneoIdIn( liga ) );

			foreach( LigaProfitsObject profitObject in ligaDao.GetLigaProfitsByLiga( liga ) )
			{
				DetalleJugador det = detalles.Single( d => d.Id == profitObject.JugadorId );
				det.CostosLigaNumber = profitObject.Fees;
				det.PremiosLigaNumber = profitObject.Premios;
			}

			return detalles;
		}

		private List<DetalleJugador> GetAllDetalleJugador(string q)
		{
			List<DetalleJugador> detalles = new List<DetalleJugador>();
			string query = string.Format( Properties.Queries.GetStats, q );
			joserrasQuery.ExecuteQuery( query, reader =>
			{
				while( reader.Read() )
				{
					detalles.Add( mapper.Map<DetalleJugador>( reader ) );
				}
			} );

			return detalles;
		}

		#endregion

	}

}