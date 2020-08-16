using AutoMapper;
using System;
using System.Data.SqlClient;
using TorneosWeb.domain.models;

namespace TorneosWeb.util.automapper
{
	public class SqlProfile : Profile
	{
		public SqlProfile()
		{
			CreateMap<SqlDataReader, Torneo>()
				.ForMember( dest => dest.Id, opt => opt.MapFrom( src => (Guid)src[ "id" ] ) )
				.ForMember( dest => dest.FechaDate, opt => opt.MapFrom( src => (DateTime)src[ "fecha" ] ) )
				.ForMember( dest => dest.PrecioBuyinNumber, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "precio_buyin" ) ) ) )
				.ForMember( dest => dest.PrecioRebuyNumber, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "precio_rebuy" ) ) ) )
				.ForMember( dest => dest.Entradas, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "entradas" ) ) ) )
				.ForMember( dest => dest.Rebuys, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "rebuys" ) ) ) )
				.ForMember( dest => dest.Bolsa, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "bolsa" ) ).ToString( Constants.CURRENCY_FORMAT ) ) )
				.ForMember( dest => dest.PremioBountyNumber, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "premio_x_bounty" ) ) ) )
				.ForMember( dest => dest.Tipo, opt => opt.MapFrom( src => src.GetFieldValue<string>( src.GetOrdinal( "tipo" ) ) ) )
				.ForMember( dest => dest.Ganador, opt => opt.MapFrom( src => src.VisibleFieldCount > 9 ? src.GetString( 9 ) : "" ) )
				.ForMember( dest => dest.GanadorId, opt => opt.MapFrom( src => src.VisibleFieldCount > 10 ? src.GetGuid( 10 ) : Guid.Empty ) )
				.IgnoreNoMap();

			CreateMap<SqlDataReader, Posicion>()
				.ForMember( dest => dest.JugadorId, opt => opt.MapFrom( src => (Guid)src[ "jugador_id" ] ) )
				.ForMember( dest => dest.Rebuys, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "rebuys" ) ) ) )
				.ForMember( dest => dest.Lugar, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "posicion" ) ) ) )
				.ForMember( dest => dest.Podio, opt => opt.MapFrom( src => src.GetFieldValue<bool>( src.GetOrdinal( "podio" ) ) ? "Podio" : "-" ) )
				.ForMember( dest => dest.PremioNumber, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "premio" ) ) ) )
				.ForMember( dest => dest.Burbuja, opt => opt.MapFrom( src => (bool)src[ "burbuja" ] ? "Burbuja" : "-" ) )
				.ForMember( dest => dest.Nombre, opt => opt.MapFrom( src => src.GetFieldValue<string>( src.GetOrdinal( "nombre" ) ) ) )
				.ForMember( dest => dest.PremioBountiesNumber, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "premio_bounties" ) ) ) )
				.ForMember( dest => dest.Knockouts, opt => opt.MapFrom( src => src.GetFieldValue<int>( src.GetOrdinal( "kos" ) ) ) )
				.ForMember( dest => dest.Puntualidad, opt => opt.MapFrom( src => src.GetFieldValue<bool>( src.GetOrdinal( "puntualidad" ) ) ) )
				.IgnoreNoMap();

			CreateMap<SqlDataReader, Knockouts>()
				.ForMember( dest => dest.Nombre, opt => opt.MapFrom( src => src.GetString( 0 ) ) )
				.ForMember( dest => dest.Eliminado, opt => opt.MapFrom( src => src.GetString( 1 ) ) )
				.ForMember( dest => dest.Eliminaciones, opt => opt.MapFrom( src => src.GetInt32( 2 ) ) );

			CreateMap<SqlDataReader, DetalleJugador>()
				.ForMember( dest => dest.Id, opt => opt.MapFrom( src => src.GetGuid( 0 ) ) )
				.ForMember( dest => dest.Nombre, opt => opt.MapFrom( src => src.GetString( 1 ) ) )
				.ForMember( dest => dest.CostosNumber, opt => opt.MapFrom( src => src.GetInt32( 2 ) ) )
				.ForMember( dest => dest.PremiosNumber, opt => opt.MapFrom( src => src.GetInt32( 3 ) ) )
				.ForMember( dest => dest.Podios, opt => opt.MapFrom( src => src.GetInt32( 4 ) ) )
				.ForMember( dest => dest.Victorias, opt => opt.MapFrom( src => src.GetInt32( 5 ) ) )
				.ForMember( dest => dest.ProfitNumber, opt => opt.MapFrom( src => src.GetInt32( 6 ) ) )
				.ForMember( dest => dest.Burbujas, opt => opt.MapFrom( src => src.GetInt32( 7 ) ) )
				.ForMember( dest => dest.Kos, opt => opt.MapFrom( src => src.GetInt32( 8 ) ) )
				.ForMember( dest => dest.Torneos, opt => opt.MapFrom( src => src.GetInt32( 9 ) ) )
				.ForMember( dest => dest.Rebuys, opt => opt.MapFrom( src => src.GetInt32( 10 ) ) )
				.IgnoreNoMap();
		}

	}

}