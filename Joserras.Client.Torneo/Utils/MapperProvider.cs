using AutoMapper;
using AutoMapper.Configuration;
using Joserras.Client.Torneo.Model;
using Joserras.Commons.Dto;
using SimpleInjector;

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
				.ForMember( d => d.Bolsa, opt => opt.Ignore() )
				.ForMember( d => d.Rebuys, opt => opt.Ignore() )
				.ForMember( d => d.PrecioRebuy, opt => opt.Ignore() )
				.ForMember( d => d.Entradas, opt => opt.Ignore() );

			mce.CreateMap<Resultado, ResultadosDTO>()
				.ForMember( d => d.TorneoId, opt => opt.Ignore() )
				.ForMember( d => d.JugadorId, opt => opt.Ignore() )
				.ForMember( d => d.Nuevo, opt => opt.Ignore() )
				.ForMember( d => d.Podio, opt => opt.Ignore() )
				.ForMember( d => d.Burbuja, opt => opt.Ignore() );

			mce.CreateMap<KO, KnockoutsDTO>();

			var mc = new MapperConfiguration( mce );
			mc.AssertConfigurationIsValid();

			IMapper m = new Mapper( mc, t => _container.GetInstance( t ) );

			return m;
		}

	}

}