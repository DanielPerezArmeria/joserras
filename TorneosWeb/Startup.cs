using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using System.Linq;
using System.Reflection;
using TorneosWeb.service;
using TorneosWeb.service.decorators;
using TorneosWeb.service.impl;
using TorneosWeb.util;
using TorneosWeb.util.automapper;

namespace TorneosWeb
{
	public class Startup
	{
		private Container container = new SimpleInjector.Container();

		public Startup(IConfiguration configuration, ILogger<Startup> logger)
		{
			container.Options.ResolveUnregisteredConcreteTypes = false;
			Configuration = configuration;
			logger.LogDebug( "********  STARTING APP  ********" );
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>( options =>
			 {
				 // This lambda determines whether user consent for non-essential cookies is needed for a given request.
				 options.CheckConsentNeeded = context => true;
				 options.MinimumSameSitePolicy = SameSiteMode.None;
			 } );

			services.AddMvc().SetCompatibilityVersion( CompatibilityVersion.Version_2_2 );

			services.AddSimpleInjector( container, options =>
			{
				options.AddAspNetCore()
					.AddControllerActivation()
					.AddPageModelActivation()
					.AddViewComponentActivation();

				options.AddLogging();
			} );

			InitializeContainer();
		}

		private void InitializeContainer()
		{
			container.RegisterSingleton<MapperProvider>();
			container.RegisterSingleton( () => GetMapper( container ) );

			RegisterNamespace( "TorneosWeb.service.impl" );

			container.RegisterDecorator<IReadService, TransactionWrapperReadService>( Lifestyle.Singleton );
			container.RegisterDecorator<IStatsService, TransactionWrapperStatsService>( Lifestyle.Singleton );

			container.RegisterSingleton<JoserrasQuery>();
		}

		private void RegisterNamespace(string nameSpace)
		{
			var registrations =
				from type in Assembly.GetExecutingAssembly().GetExportedTypes()
				where type.Namespace.StartsWith( nameSpace )
				from service in type.GetInterfaces()
				select new { service, implementation = type };

			foreach( var reg in registrations )
			{
				container.Register( reg.service, reg.implementation, Lifestyle.Singleton );
			}
		}

		private IMapper GetMapper(Container container)
		{
			var mp = container.GetInstance<MapperProvider>();
			return mp.GetMapper();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseSimpleInjector( container );

			if( env.IsDevelopment() )
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler( "/Error" );
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseMvc();

			container.Verify();
		}

	}

}