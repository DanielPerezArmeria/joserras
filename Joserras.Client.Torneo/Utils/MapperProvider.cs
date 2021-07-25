using AutoMapper;
using AutoMapper.Configuration;
using Joserras.Client.Torneo.Model;
using Joserras.Commons.Dto;
using SimpleInjector;
using System;
using System.Linq.Expressions;

namespace Joserras.Client.Torneo.Utils
{
	public class MapperProvider
	{
		private readonly Container _container;

		public MapperProvider(Container container)
		{
			_container = container;
		}

		public IMapper GetMapper()
		{
			var mce = new MapperConfigurationExpression();
			mce.ConstructServicesUsing( _container.GetInstance );

			mce.CreateMap<TorneoViewModel, TorneoDTO>()
				.Ignore( rec => rec.Bolsa )
				.Ignore( rec => rec.Entradas )
				.Ignore( rec => rec.PrecioBounty )
				.Ignore( rec => rec.Premiacion )
				.Ignore( rec => rec.Rebuys )
				.Ignore( rec => rec.PrecioRebuy );

			mce.CreateMap<Resultado, ResultadosDTO>()
				.Ignore( rec => rec.Burbuja )
				.Ignore( rec => rec.JugadorId )
				.Ignore( rec => rec.Kos )
				.Ignore( rec => rec.Nuevo )
				.Ignore( rec => rec.Podio )
				.Ignore( rec => rec.TorneoId );

			mce.CreateMap<KO, KnockoutsDTO>();

			var mc = new MapperConfiguration( mce );
			mc.AssertConfigurationIsValid();

			IMapper m = new Mapper( mc, t => _container.GetInstance( t ) );

			return m;
		}

	}

	public static class AutomapperExtension
	{
		public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
			this IMappingExpression<TSource, TDestination> map,
			Expression<Func<TDestination, object>> selector)
		{
			map.ForMember( selector, config => config.Ignore() );
			return map;
		}
	}

}