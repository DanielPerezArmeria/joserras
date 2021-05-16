using AutoMapper;
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
using TorneosWeb.config;
using TorneosWeb.dao;
using TorneosWeb.dao.decorators;
using TorneosWeb.dao.impl;
using TorneosWeb.service;
using TorneosWeb.service.decorators;
using TorneosWeb.service.impl;
using TorneosWeb.util;
using TorneosWeb.util.automapper;
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

			services.Configure<AzureTableConfig>( Configuration.GetSection( "AzureTableConfig" ) );

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
			List<Type> ignoreClasses = new List<Type>();

			container.RegisterSingleton<MapperProvider>();
			container.RegisterSingleton( () => GetMapper( container ) );

			container.RegisterSingleton<IProfitsExporter>( () =>
				new GoogleSheetsProfitsExporter( contentRoot + @"/Files/Joserras Project-fd7d4368e5dd.json",
				"1fWhxbneW19urTN7RFMaTRtycIB32X6C4LPM9QGzDY-w", container.GetInstance<ILogger<GoogleSheetsProfitsExporter>>(),
				container.GetInstance<IReadService>(), container.GetInstance<ILigaReader>() ) );

			ignoreClasses.Add( Type.GetType( "TorneosWeb.service.IProfitsExporter" ) );

			RegisterNamespace( "TorneosWeb.service.impl", ignoreClasses );
			RegisterNamespace( "TorneosWeb.dao.impl" );

			container.RegisterDecorator<IReadService, CacheWrapperReadService>( Lifestyle.Singleton );
			container.RegisterDecorator<IStatsService, CacheWrapperStatsService>( Lifestyle.Singleton );
			container.RegisterDecorator<ILigaReader, CacheWrapperLigaReader>( Lifestyle.Singleton );
			container.RegisterDecorator<IWriteService, WriteServiceLigaDecorator>( Lifestyle.Singleton );
			container.RegisterDecorator<IWriteService, BalanceGeneratorWriteServiceDecorator>( Lifestyle.Singleton );
			container.RegisterDecorator( typeof( IStandingsDao<> ), typeof( NullAzureDaoDecorator<> ), Lifestyle.Singleton );

			container.RegisterSingleton( typeof( IStandingsDao<> ), typeof( StandingsAzureDao<> ) );

			container.Collection.Register<IPrizeFiller>( new[] { typeof( IPrizeFiller ).Assembly }, Lifestyle.Singleton );

			container.RegisterSingleton<JoserrasQuery>();
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

		private IMapper GetMapper(Container container)
		{
			var mp = container.GetInstance<MapperProvider>();
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