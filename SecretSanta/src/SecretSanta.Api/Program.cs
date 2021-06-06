using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace SecretSanta.Api
{
    public class Program
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        private static Serilog.ILogger Logger { get; set; }

        public static void Main(string[] args)
        {
            // adapted from https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/

            string template = "{Timestamp} [{Level:u4}] ({Application}: {SourceContext}) {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "API")
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: template,
                    theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    "log.txt",
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: template)
                .CreateLogger();

            Logger = Log.Logger.ForContext<Program>();

            try
            {
                Logger.Information("Startup");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Startup Failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
