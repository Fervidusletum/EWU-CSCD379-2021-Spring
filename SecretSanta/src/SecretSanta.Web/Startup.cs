using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SecretSanta.Web.Api;

namespace SecretSanta.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public System.Net.Http.HttpClient UsersHttpClient { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            UsersHttpClient = new()
            {
                BaseAddress = new Uri(Configuration["ApiHost"])
            };
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUsersClient, UsersClient>(_ => new UsersClient(UsersHttpClient));
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
