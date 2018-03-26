using Autofac.Extensions.DependencyInjection;
using Boondocks.Base.Auth.Core;
using Boondocks.Device.WebApi.Authentication;
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

namespace Boondocks.Device.WebApi
{
    // Configures the HTTP request pipeline and bootstraps the NetFusion 
    // application.
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc(options => {
                options.UseHalFormatter();
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                   "RequireAuthenticatedUser",                  
                   policyBuilder => {
                       policyBuilder.RequireAuthenticatedUser();
                   });
            });
            
           services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddDeviceAuthentication<DeviceAuthService>(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                   
                });

             services.AddMvc(options => {
                options.Filters.Add(new AuthorizeFilter("RequireAuthenticatedUser"));
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


            // Inserts the filter to make sure all HTTP requests are authenticated.
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseCors(builder => builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
                    
                app.UseDeveloperExceptionPage();
                app.UseCompositeQuerying();
            }

            // Adds MVC components to the pipe-line.  Common ASP.NET Core call.
            app.UseMvc();
        }

        private void OnShutdown()
        {
            AppContainer.Instance.Stop();
        }
    }
}

