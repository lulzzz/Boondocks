﻿using System.IO;
using Autofac;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Boondocks.Services.Management.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Boondocks Management API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Boondocks.Services.Management.WebApi.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Boondocks Management API V1");
            });

            app.UseMvc();
        }
     
        // This is the default if you don't have an environment specific method.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add things to the Autofac ContainerBuilder.
            builder.RegisterInstance(new SqlServerDbConnectionFactory(@"Server=localhost\sqlexpress;Database=Boondocks;User Id=boondocks;Password=#Px@S:w_j+V97ngz;"))
                .As<IDbConnectionFactory>()
                .SingleInstance();
        }
    }
}
