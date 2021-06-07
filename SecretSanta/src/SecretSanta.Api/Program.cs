using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace SecretSanta.Api
{
    public class Program
    {
        private static Serilog.ILogger Logger { get; set; }
        private static string Template { get; } = "[{Timestamp} {Level:u4}] ({Category}: {SourceContext}) {Message:lj}{NewLine}{Exception}";
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            string execDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            Dictionary<string, string> defaultEnvironmentVariables = new()
            {
                { "Config:DbName", "main.db" },
                { "Config:DbPath", @"..\SecretSanta.Data\" }, // set to assembly folder?
                { "Config:LogName", "db.log" },
                { "Config:LogPath", execDir }
            };

            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(defaultEnvironmentVariables)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            IConfigurationRoot config = builder.Build();
            Configuration = config.GetSection("Config");


            // adapted from https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/
            // part 1 of 2 stage init, used to log startup https://github.com/serilog/serilog-aspnetcore#two-stage-initialization
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Category", "API")
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

            foreach (IConfigurationSection evar in Configuration.GetChildren())
            {
                Logger.Information("Env Var {EVKey} is {EVVal}", evar.Key ?? "NULL", evar.Value ?? "NULL");
            }

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
                // part 2 of 2 stage init, completely overwrites stage1
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Debug()
                    .Enrich.WithProperty("Category", "API")
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
