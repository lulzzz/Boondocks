using System.IO;
using Autofac;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.WebApi.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using MongoDB.Driver;
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

            // https://stackoverflow.com/a/40364756/232566
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Boondocks Management API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Boondocks.Services.Management.WebApi.xml");
                c.IncludeXmlComments(xmlPath);

                c.OperationFilter<FileUploadOperation>();
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
     
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            //Deal with the configuration bits
            var config = configBuilder.Build();

            builder.RegisterInstance(config);

            string dbConnectionString = config["DbConnectionString"];

            var registryConfig = new RegistryConfig();
            var provisioningConfig = new ProvisioningConfig();

            config.GetSection("registry").Bind(registryConfig);
            config.GetSection("provisioningConfig").Bind(provisioningConfig);

            builder.RegisterInstance(registryConfig);
            builder.RegisterInstance(provisioningConfig);
                
            // Add things to the Autofac ContainerBuilder.
            builder.RegisterInstance(new SqlServerDbConnectionFactory(dbConnectionString))
                .As<IDbConnectionFactory>()
                .SingleInstance();

            //blob config
            builder.RegisterInstance(new BlobDataAccessConfiguration("mongodb://localhost", "Boondocks"))
                .As<IBlobDataAccesConfiguration>();

            //blob access
            builder.RegisterType<BlobDataAccessProvider>()
                .As<IBlobDataAccessProvider>()
                .SingleInstance();
        }
    }
}
