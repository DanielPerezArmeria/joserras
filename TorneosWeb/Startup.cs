using AutoMapper;
using Joserras.Commons.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TorneosWeb.dao;
using TorneosWeb.dao.decorators;
using TorneosWeb.dao.impl;
using TorneosWeb.Properties;
using TorneosWeb.service;
using TorneosWeb.service.decorators;
using TorneosWeb.util.automapper;
using TorneosWeb.util.disqualifiers;
using TorneosWeb.util.prize;
using TorneosWeb.util.schedule;

namespace TorneosWeb
{
	public class Startup
	{
		private Container container = new();
		private string contentRoot;
		private readonly ILogger<Startup> log;

		public Startup(IConfiguration configuration, ILogger<Startup> logger)
		{
			container.Options.ResolveUnregisteredConcreteTypes = false;
			Configuration = configuration;
			contentRoot = configuration.GetValue<string>( WebHostDefaults.ContentRootKey );
			log = logger;
			log.LogInformation( "********  STARTING APP  ********" );
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();

			CultureInfo mex = new( "es-MX" );
			CultureInfo.DefaultThreadCurrentCulture = mex;
			CultureInfo.DefaultThreadCurrentUICulture = mex;

			services.Configure<CookiePolicyOptions>( options =>
			 {
				 // This lambda determines whether user consent for non-essential cookies is needed for a given request.
				 options.CheckConsentNeeded = context => true;
				 options.MinimumSameSitePolicy = SameSiteMode.None;
			 } );

			services.AddSimpleInjector( container, options =>
			{
				options.AddAspNetCore()
					.AddControllerActivation()
					.AddPageModelActivation()
					.AddViewComponentActivation();

				options.AddLogging();
			} );

			InitializeContainer();

			bool autoCreateBalance = Configuration.GetValue<bool>( "CreateBalance" );
			log.LogInformation( "Auto-create balance sheet: {0}", autoCreateBalance );
			if( autoCreateBalance )
			{
				services.AddScheduler( builder =>
				 {
					 builder.Services.AddSingleton( s => container.GetInstance<IReadService>() );
					 builder.Services.AddSingleton( s => container.GetInstance<IProfitsExporter>() );
					 builder.Services.AddSchedulerJob<CreateBalanceJob, CreateBalanceJobOptions>();
				 } );
			}
		}

		private void InitializeContainer()
		{
			List<Type> ignoreClasses = new();

			container.RegisterSingleton<MapperProvider>();
			container.RegisterSingleton( () => GetMapper( container ) );

			RegisterNamespace( "TorneosWeb.service.impl" );
			RegisterNamespace( "TorneosWeb.dao.impl" );
			RegisterConfigSections();

			container.RegisterSingleton( typeof( IStandingsDao<> ), typeof( StandingsAzureDao<> ) );

			container.Collection.Register<IPrizeFiller>( new[] { typeof( IPrizeFiller ).Assembly }, Lifestyle.Singleton );

			container.Collection.Register<IDisqualifier>( new[] { typeof( IDisqualifier ).Assembly }, Lifestyle.Singleton );

			container.RegisterSingleton(
				() => new JoserrasQuery( container.GetInstance<IConfiguration>().GetConnectionString( Resources.joserrasDb ) )
			);

			container.RegisterDecorator<IReadService, CacheWrapperReadService>( Lifestyle.Singleton );
			container.RegisterDecorator<IReadService, LockingReadServiceDecorator>( Lifestyle.Singleton );
			container.RegisterDecorator<IStatsService, CacheWrapperStatsService>( Lifestyle.Singleton );
			container.RegisterDecorator<IStatsService, LockingStatsServiceDecorator>( Lifestyle.Singleton );
			container.RegisterDecorator<ILigaReader, CacheWrapperLigaReader>( Lifestyle.Singleton );
			container.RegisterDecorator<ILigaReader, LockingLigaReaderDecorator>( Lifestyle.Singleton );
			container.RegisterDecorator<IWriteService, WriteServiceLigaDecorator>( Lifestyle.Singleton );
			container.RegisterDecorator<IWriteService, BalanceGeneratorWriteServiceDecorator>( Lifestyle.Singleton );
			container.RegisterDecorator<IJugadorService, CacheWrappedJugadorService>( Lifestyle.Singleton );
			container.RegisterDecorator( typeof( IStandingsDao<> ), typeof( NullAzureDaoDecorator<> ), Lifestyle.Singleton );
		}

		private void RegisterNamespace(string nameSpace, IEnumerable<Type> ignoreServiceClasses)
		{
			var registrations =
				from type in Assembly.GetExecutingAssembly().GetExportedTypes()
				where type.Namespace.StartsWith( nameSpace ) && !type.IsGenericType
				from service in type.GetInterfaces()
				select new { service, implementation = type };

			foreach( var reg in registrations )
			{
				if( ignoreServiceClasses.Contains( reg.service ) )
				{
					continue;
				}
				container.Register( reg.service, reg.implementation, Lifestyle.Singleton );
			}
		}

		private void RegisterNamespace(string nameSpace)
		{
			RegisterNamespace( nameSpace, new List<Type>() );
		}

		private void RegisterConfigSections()
		{
			string configNamespace = "TorneosWeb.config";

			IEnumerable<Type> registrations =
				from type in Assembly.GetExecutingAssembly().GetExportedTypes()
				where type.Namespace.StartsWith( configNamespace )
				select type;

			foreach(Type t in registrations )
			{
				container.RegisterInstance( t, Configuration.GetSection( t.Name ).Get( t ) );
			}

		}

		private IMapper GetMapper(Container container)
		{
			MapperProvider mp = container.GetInstance<MapperProvider>();
			return mp.GetMapper();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseSerilogRequestLogging();
			app.UseSimpleInjector( container );

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			try
			{
				container.Verify();
			}
			catch (Exception e)
			{
				log.LogError( e, e.Message );
				throw;
			}
		}

	}

}