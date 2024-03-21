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
			string outputFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] "
				+ "({SourceContext}) {Message:lj}{NewLine}{Exception}";
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.Enrich.FromLogContext()
				.WriteTo.Console(outputTemplate: outputFormat)
				.WriteTo.File( "joserras-.log", rollingInterval: RollingInterval.Month,
					outputTemplate: outputFormat )
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