using AutoMapper;
using AutoMapper.Configuration;
using SimpleInjector;

namespace TorneosWeb.util.automapper
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

			mce.AddMaps( typeof( SqlProfile ).Assembly );

			var mc = new MapperConfiguration( mce );
			mc.AssertConfigurationIsValid();

			IMapper m = new Mapper( mc, t => _container.GetInstance( t ) );

			return m;
		}

	}

}