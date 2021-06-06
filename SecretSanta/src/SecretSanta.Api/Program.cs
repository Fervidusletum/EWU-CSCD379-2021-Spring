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
        private static string Template { get; } = "[{Timestamp} {Level:u4}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";

        public static void Main(string[] args)
        {
            // adapted from https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: Template,
                    theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    "log.txt",
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: Template)
                .CreateBootstrapLogger();

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
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        outputTemplate: Template,
                        theme: AnsiConsoleTheme.Code)
                    .WriteTo.File(
                        "log.txt",
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        outputTemplate: Template)
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
