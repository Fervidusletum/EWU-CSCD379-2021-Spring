using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecretSanta.Data;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Linq;
using DbContext = SecretSanta.Data.DbContext;

namespace SecretSanta.Api
{
    public class Program
    {
        private static string Template { get; } = "[{Timestamp} {Level:u4}] ({Category}: {SourceContext}) {Message:lj}{NewLine}{Exception}";
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            //string execDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            Dictionary<string, string> defaultEnvironmentVariables = new()
            {
                { "Config:DbName", "main.db" },
                { "Config:DbPath", @"..\SecretSanta.Data\" }, // set to assembly folder?
                { "Config:LogName", "db.log" },
                { "Config:LogPath", @"..\SecretSanta.Data\" },
                { "Config:DeploySampleData", args.Any(arg => arg.Contains("DeploySampleData")) ? bool.TrueString : bool.FalseString }
            };

            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(defaultEnvironmentVariables)
                .AddEnvironmentVariables()
                .AddCommandLine(args,new Dictionary<string, string> {
                    { "--DbName", "Config:DbName" },
                    { "--DbPath", "Config:DbPath" }
                });

            IConfigurationRoot config = builder.Build();
            Configuration = config.GetSection("Config");


            // adapted from https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/
            // part 1 of 2 stage init, used to log startup https://github.com/serilog/serilog-aspnetcore#two-stage-initialization
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Category", "API")
                .MinimumLevel.Information()
                .WriteTo.Console(
                    outputTemplate: Template,
                    theme: AnsiConsoleTheme.Code)
                .WriteTo.File(
                    "log.txt",
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: Template)
                .CreateBootstrapLogger();

            Serilog.ILogger Logger = Log.Logger.ForContext<Program>().ForContext("Category", "Startup");

            foreach (string arg in args)
            {
                Logger.Information("Arg found: {Arg}", arg);
            }

            foreach (IConfigurationSection evar in Configuration.GetChildren())
            {
                Logger.Information("Env Var {EVKey} is {EVVal}", evar.Key ?? "NULL", evar.Value ?? "NULL");
            }

            try
            {
                Logger.Information("Building host");
                CreateHostBuilder(args)
                    .Build()
                    .MigrateDatabase(Convert.ToBoolean(Configuration.GetValue<string>("DeploySampleData")))
                    .Run();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Host build Failed");
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
                    .MinimumLevel.Information()
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
