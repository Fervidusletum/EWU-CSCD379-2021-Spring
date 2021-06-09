using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SecretSanta.Api.Tests.Business;
using SecretSanta.Business;
using Serilog;

namespace SecretSanta.Api.Tests
{
    internal class WebApplicationFactory : WebApplicationFactory<Startup>
    {
        internal TestableUserRepository UserRepository { get; } = new();
        internal TestableGroupRepository GroupRepository { get; } = new();
        internal TestableGiftRepository GiftRepository { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => {
                services.AddScoped<IUserRepository, TestableUserRepository>(_ => UserRepository);
                services.AddScoped<IGroupRepository, TestableGroupRepository>(_ => GroupRepository);
                services.AddScoped<IGiftRepository, TestableGiftRepository>(_ => GiftRepository);
            });
        }
    }
}
