using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SecretSanta.E2E
{
    public abstract class WebHostServerFixture : IDisposable
    {
        public readonly Lazy<Uri> WebRootUriInitializer;
        public Uri WebRootUri => WebRootUriInitializer.Value;
        public IHost WebHost { get; set; }

        public readonly Lazy<Uri> ApiRootUriInitializer;
        public Uri ApiRootUri => ApiRootUriInitializer.Value;
        public IHost ApiHost { get; set; }

        public WebHostServerFixture()
        {
            ApiRootUriInitializer = new Lazy<Uri>(() => new Uri(StartAndGetApiRootUri()));
            WebRootUriInitializer = new Lazy<Uri>(() => new Uri(StartAndGetWebRootUri()));
        }

        protected static void RunInBackgroundThread(Action action)
        {
            using var isDone = new ManualResetEvent(false);

            ExceptionDispatchInfo edi = null;
            new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    edi = ExceptionDispatchInfo.Capture(ex);
                }

                isDone.Set();
            }).Start();

            if (!isDone.WaitOne(TimeSpan.FromSeconds(10)))
                throw new TimeoutException("Timed out waiting for: " + action);

            if (edi != null)
                throw edi.SourceException;
        }

        protected virtual string StartAndGetWebRootUri()
        {
            // As the port is generated automatically, we can use IServerAddressesFeature to get the actual server URL
            WebHost = CreateWebHost();
            RunInBackgroundThread(WebHost.Start);
            return WebHost.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()
                .Addresses.Single();
        }

        protected virtual string StartAndGetApiRootUri()
        {
            // As the port is generated automatically, we can use IServerAddressesFeature to get the actual server URL
            ApiHost = CreateApiHost();
            RunInBackgroundThread(ApiHost.Start);
            return ApiHost.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()
                .Addresses.Single();
        }

        public virtual void Dispose()
        {
            WebHost?.Dispose();
            WebHost?.StopAsync();
            ApiHost?.Dispose();
            ApiHost?.StopAsync();
        }

        protected abstract IHost CreateWebHost();
        protected abstract IHost CreateApiHost();
    }

    public class WebHostServerFixture<WStartup, AStartup> : WebHostServerFixture
    where WStartup : class
    where AStartup : class
    {
        protected override IHost CreateWebHost()
        {
            return new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    // Make UseStaticWebAssets work
                    var applicationPath = typeof(WStartup).Assembly.Location;
                    var applicationDirectory = Path.GetDirectoryName(applicationPath);
                    var name = Path.ChangeExtension(applicationPath, ".StaticWebAssets.xml");

                    var inMemoryConfiguration = new Dictionary<string, string>
                    {
                        [WebHostDefaults.StaticWebAssetsKey] = name,
                    };

                    config.AddInMemoryCollection(inMemoryConfiguration);
                })
                .ConfigureWebHost(webHostBuilder => webHostBuilder
                    .UseKestrel()
                    .UseSolutionRelativeContentRoot(Path.Combine("src", typeof(WStartup).Assembly.GetName().Name))
                    .UseStaticWebAssets()
                    .UseStartup<WStartup>()
                    .UseSetting("ApiHost", ApiRootUri.AbsoluteUri.Replace("127.0.0.1", "localhost"))
                    .UseUrls($"https://127.0.0.1:0")) // :0 allows to choose a port automatically
                .Build();
        }

        protected override IHost CreateApiHost()
        {
            return new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    // Make UseStaticWebAssets work
                    var applicationPath = typeof(AStartup).Assembly.Location;
                    var applicationDirectory = Path.GetDirectoryName(applicationPath);
                    var name = Path.ChangeExtension(applicationPath, ".StaticWebAssets.xml");

                    var inMemoryConfiguration = new Dictionary<string, string>
                    {
                        [WebHostDefaults.StaticWebAssetsKey] = name,
                    };

                    config.AddInMemoryCollection(inMemoryConfiguration);
                })
                .ConfigureWebHost(webHostBuilder => webHostBuilder
                    .UseKestrel()
                    .UseSolutionRelativeContentRoot(Path.Combine("src", typeof(AStartup).Assembly.GetName().Name))
                    .UseStaticWebAssets()
                    .UseStartup<AStartup>()
                    .UseUrls($"https://127.0.0.1:0")) // :0 allows to choose a port automatically
                .Build();
        }
    }
}
