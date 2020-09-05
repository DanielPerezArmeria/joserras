using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace TorneosWeb
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File( "joserras-.log", rollingInterval: RollingInterval.Month )
				.MinimumLevel.Override( "Microsoft.AspNetCore", LogEventLevel.Warning )
				.CreateLogger();
			CreateWebHostBuilder( args ).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
				WebHost.CreateDefaultBuilder( args )
						.ConfigureLogging( logging =>
						{
							logging.ClearProviders();
							logging.AddConsole();
						} )
						.UseStartup<Startup>()
						.UseSerilog();
	}

}