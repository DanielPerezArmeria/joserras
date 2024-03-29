﻿using AutoMapper;
using Joserras.Client.Torneo.Service;
using Joserras.Client.Torneo.Service.Creators;
using Joserras.Client.Torneo.Service.Impl;
using Joserras.Client.Torneo.Service.Senders;
using Joserras.Client.Torneo.Utils;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Joserras.Client.Torneo
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private Container container;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			container = new();

			ConfigureLogging();

			container.RegisterSingleton<MapperProvider>();
			container.RegisterSingleton( () => GetMapper( container ) );

			RegisterConcreteNamespace( "Joserras.Client.Torneo.Model" );

			container.RegisterSingleton<ITournamentCreator, JsonTournamentCreator>();
			container.RegisterSingleton<ITournamentSender, JsonTournamentSender>();

			container.Collection.Register<ITorneoValidator>( new[] { typeof( ITorneoValidator ).Assembly }, Lifestyle.Singleton );

			container.RegisterSingleton<IJoserrasService, JoserrasService>();
			container.RegisterSingleton<IHttpService>( () => new HttpService( @"https://joserras.azurewebsites.net/",
				container.GetInstance<ILogger<HttpService>>() ) );
			/*container.RegisterSingleton<IHttpService>( () => new HttpService( @"https://localhost:5001/",
				container.GetInstance<ILogger<HttpService>>() ) );*/

			container.RegisterSingleton<Startup>();
			container.Register<MainWindow>();

			try
			{
				container.Verify();
			}
			catch( InvalidOperationException ioe )
			{
				Log.Error( "{0}", ioe );
				throw;
			}

			container.GetInstance<Startup>().Run();
		}

		private void RegisterNamespace(string nameSpace)
		{
			RegisterNamespace( nameSpace, new List<Type>() );
		}

		private void RegisterNamespace(string nameSpace, IEnumerable<Type> ignoreServiceClasses)
		{
			var registrations =
				from type in Assembly.GetExecutingAssembly().GetExportedTypes()
				where type.Namespace.StartsWith( nameSpace ) && !type.IsGenericType
				from service in type.GetInterfaces()
				select new { service, implementation = type };

			foreach (var reg in registrations)
			{
				if (ignoreServiceClasses.Contains( reg.service ))
				{
					continue;
				}
				container.Register( reg.service, reg.implementation, Lifestyle.Singleton );
			}
		}

		private void RegisterConcreteNamespace(string nameSpace)
		{
			var registrations =
				from type in Assembly.GetExecutingAssembly().GetExportedTypes()
				where type.Namespace.StartsWith( nameSpace ) && !type.IsGenericType
				select new { implementation = type };

			foreach (var reg in registrations)
			{
				container.Register( reg.implementation, reg.implementation, Lifestyle.Singleton );
			}
		}

		private void ConfigureLogging()
		{
			container.RegisterSingleton( CreateLoggerFactory );
			container.Register( typeof( ILogger<> ), typeof( Logger<> ), Lifestyle.Singleton );
		}

		private ILoggerFactory CreateLoggerFactory()
		{
			LoggerFactory factory = new LoggerFactory();

			Serilog.ILogger logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File( "logs/joserras-.log", rollingInterval: RollingInterval.Month )
				.MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
				.CreateLogger();

			factory.AddSerilog( logger );

			return factory;
		}

		private IMapper GetMapper(Container container)
		{
			var mp = container.GetInstance<MapperProvider>();
			return mp.GetMapper();
		}

	}

}