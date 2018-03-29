using Autofac.Extensions.DependencyInjection;
using Boondocks.Api.WebApi.Bootstrap;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFusion.Bootstrap.Container;
using NetFusion.Rest.Server.Hal;
using NetFusion.Web.Mvc.Composite;
using System;

namespace Boondocks.Auth.WebApi
{
    // Configures the HTTP request pipeline and bootstraps the NetFusion 
    // application container.
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IConfiguration configuration, ILoggerFactory loggingFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggingFactory;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Support REST/HAL based API responses.
            services.AddMvc(options => {
                options.UseHalFormatter();
            });

            // Configure NetFusion Application Container
            AppContainerSetup.Bootstrap(_configuration, _loggerFactory, services);

            // Return instance of dependency container to be used
            return new AutofacServiceProvider(AppContainer.Instance.Services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime)
        { 
            // This registers a method to be called when the Web Application is stopped.
            // In this case, we want to delegate to the NetFusion AppContainer so it can
            // safely stopped.
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            if (env.IsDevelopment())
            {
                string viewerUrl = _configuration.GetValue<string>("Startup:Netfusion:ViewerUrl");
                if (! string.IsNullOrWhiteSpace(viewerUrl))
                {
                    app.UseCors(builder => builder.WithOrigins(viewerUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader());
                }

                app.UseDeveloperExceptionPage();
                app.UseCompositeQuerying();
            }

            app.UseAuthentication();
            app.UseMvc();
        }

        private void OnShutdown()
        {
            AppContainer.Instance.Stop();
        }
    }
}

