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
				.ForMember( dest => dest.Id, opt => opt.MapFrom( src => src.GetGuid( 0 ) ) )
				.ForMember( dest => dest.Fecha, opt => opt.MapFrom( src => src.GetDateTime( 1 ).ToString( "dd 'de' MMMM yyyy" ) ) )
				.ForMember( dest => dest.Precio_Buyin, opt => opt.MapFrom( src => src.GetInt32( 2 ).ToString( "$###,###" ) ) )
				.ForMember( dest => dest.Precio_Rebuy, opt => opt.MapFrom( src => src.GetInt32( 3 ).ToString( "$###,###" ) ) )
				.ForMember( dest => dest.Entradas, opt => opt.MapFrom( src => src.GetInt32( 4 ) ) )
				.ForMember( dest => dest.Rebuys, opt => opt.MapFrom( src => src.GetInt32( 5 ) ) )
				.ForMember( dest => dest.Bolsa, opt => opt.MapFrom( src => src.GetInt32( 6 ).ToString( "$###,###" ) ) )
				.ForMember( dest => dest.Ganador, opt => opt.MapFrom( src => src.VisibleFieldCount > 7 ? src.GetString( 7 ) : "" ) )
				.ForMember( dest => dest.GanadorId, opt => opt.MapFrom( src => src.VisibleFieldCount > 8 ? src.GetGuid( 8 ) : Guid.Empty ) );

			CreateMap<SqlDataReader, Posicion>()
				.ForMember( dest => dest.JugadorId, opt => opt.MapFrom( src => src.GetGuid( 1 ) ) )
				.ForMember( dest => dest.Rebuys, opt => opt.MapFrom( src => src.GetInt32( 2 ) ) )
				.ForMember( dest => dest.Lugar, opt => opt.MapFrom( src => src.GetInt32( 3 ) ) )
				.ForMember( dest => dest.Podio, opt => opt.MapFrom( src => src.GetBoolean( 4 ) ? "Podio" : "-" ) )
				.ForMember( dest => dest.Premio, opt => opt.MapFrom( src => src.GetInt32( 5 ) > 0 ? src.GetInt32( 5 ).ToString( "$###,###" ) : "-" ) )
				.ForMember( dest => dest.Burbuja, opt => opt.MapFrom( src => src.GetBoolean( 6 ) ? "Burbuja" : "-" ) )
				.ForMember( dest => dest.Nombre, opt => opt.MapFrom( src => src.GetString( 7 ) ) )
				.ForMember( dest => dest.Knockouts, opt => opt.Ignore() )
				.ForMember( dest => dest.Profit, opt => opt.Ignore() )
				.ForMember( dest => dest.ProfitNumber, opt => opt.Ignore() );

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