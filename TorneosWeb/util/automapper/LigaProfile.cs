using AutoMapper;
using System;
using System.Data.SqlClient;
using TorneosWeb.domain.models.ligas;

namespace TorneosWeb.util.automapper
{
	public class LigaProfile : Profile
	{
		public LigaProfile()
		{
			CreateMap<SqlDataReader, Liga>()
				.ForMember( dest => dest.Id, opt => opt.MapFrom( src => (Guid)src[ "id" ] ) )
				.ForMember( dest => dest.Nombre, opt => opt.MapFrom( src => src.GetFieldValue<string>( src.GetOrdinal( "nombre" ) ) ) )
				.ForMember( dest => dest.Puntaje, opt => opt.MapFrom( src => src.GetFieldValue<string>( src.GetOrdinal( "puntaje" ) ) ) )
				.ForMember( dest => dest.Abierta, opt => opt.MapFrom( src => src.GetFieldValue<bool>( src.GetOrdinal( "abierta" ) ) ) )
				.ForMember( dest => dest.FechaInicioDate, opt => opt.MapFrom( src => ((DateTime)src[ "fecha_inicio" ]) ) )
				.ForMember( dest => dest.FechaCierreDate, opt => opt.MapFrom( src => src[ "fecha_cierre" ] as DateTime? ) )
				.IgnoreNoMap();
		}
	}

}