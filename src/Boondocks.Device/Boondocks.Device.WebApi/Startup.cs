using Autofac.Extensions.DependencyInjection;
using Boondocks.Base.Auth.Core;
using Boondocks.Device.WebApi.Bootstrap;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFusion.Bootstrap.Container;
using NetFusion.Rest.Server.Hal;
using NetFusion.Web.Mvc.Composite;
using System;
using Boondocks.Base.Auth;

namespace Boondocks.Device.WebApi
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Required by Boondocks.Base.Auth to allow access to the current HTTP Context.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Support REST/HAL based API responses.
            services.AddMvc(options => {
                options.UseHalFormatter();
            });

            var deviceAuthOptions = _configuration.GetDeviceOptions();

            // Create policy that will require access to all controllers to be authenticated.
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                   "RequireAuthenticatedCaller",                  
                   policyBuilder => {
                       policyBuilder.RequireAuthenticatedUser();
                   });
            });
           
            // Add authentication handler that will verify the caller as a valid signed device token.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddDeviceTokenAuth(deviceAuthOptions);

            // Add filter to request pipeline containing policy,
            services.AddMvc(options => {
                options.Filters.Add(new AuthorizeFilter("RequireAuthenticatedCaller"));
            });

            // Configure NetFusion Application Container
            AppContainerSetup.Bootstrap(_configuration, _loggerFactory, services);

            // Return instance of dependency container to be used
            return new AutofacServiceProvider(AppContainer.Instance.Services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime)
        { 
            // This registers a method to be called when the Web Application is stopped.
            // In this case, we want to delegate to the NetFusion AppContainer so it can
            // safely stopped.
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            app.UseAuthentication();
            app.UseMvc();

            if (env.IsDevelopment())
            {
                string viewerUrl = _configuration.GetValue<string>("Startup:Netfusion:ViewerUrl");
                if (!string.IsNullOrWhiteSpace(viewerUrl))
                {
                    app.UseCors(builder => builder.WithOrigins(viewerUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader());
                }

                app.UseDeveloperExceptionPage();
                app.UseCompositeQuerying();
            }


        }

        private void OnShutdown()
        {
            AppContainer.Instance.Stop();
        }
    }
}

