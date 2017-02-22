using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Canducci.WebCore.Models;
using Canducci.DocumentDBCore;

namespace Canducci.WebCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Add Options
            services.AddOptions();

            // Add Mvc.
            services.AddMvc();

            // IOC.
            IConfigurationSection config = Configuration.GetSection("DocumentDB");
            var url = config.GetValue<string>("url");
            var key = config.GetValue<string>("key");
            var database = config.GetValue<string>("database");

            Func<IServiceProvider, ConnectionDocumentDB> co =
                p => new ConnectionDocumentDB(url, key, database);
            services.AddScoped(co);
            services.AddScoped<RepositoryJornalAbstract, RepositoryJornal>();

            //IConfigurationSection config = Configuration.GetSection("DocumentDB");
            //var url = config.GetValue<string>("url");
            //var key = config.GetValue<string>("key");
            //var database = config.GetValue<string>("database");
            //services.Configure<ConfigurationDocumentDB>(o =>
            //{
            //    o.Url = url;
            //    o.Key = key;
            //    o.Database = database;
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
