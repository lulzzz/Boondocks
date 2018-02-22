namespace Boondocks.Services.Device.WebApi
{
    using System.IO;
    using System.Reflection;
    using Authentication;
    using Autofac;
    using Base;
    using DataAccess;
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Services.Contracts;
    using Swashbuckle.AspNetCore.Swagger;

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
            services.AddAuthentication(options => { options.DefaultScheme = "DeviceBearerToken"; })
                .AddCustomAuthentication("DeviceBearerToken", "Device Authentication Scheme", o => { });

            services.AddLogging(loggingBuilder =>
            {
                //loggingBuilder.AddSeq();
                loggingBuilder.AddConsole(options => { options.IncludeScopes = true; });

                loggingBuilder.AddDebug();
            });

            services.AddMvc(o => { });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Boondocks Device API", Version = "v1"});

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Boondocks.Services.Device.WebApi.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "Boondocks Device API V1"); });

            app.UseMvc();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            //Deal with the configuration bits
            var config = configBuilder.Build();

            builder.RegisterInstance(config);

            string dbConnectionString = config["DbConnectionString"];

            // Add things to the Autofac ContainerBuilder.
            builder.RegisterInstance(new SqlServerDbConnectionFactory(dbConnectionString))
                .As<IDbConnectionFactory>()
                .SingleInstance();

            var registryConfig = new RegistryConfig();

            config.GetSection("registry").Bind(registryConfig);

            builder.RegisterInstance(registryConfig);
            
            builder.RegisterType<RepositoryNameFactory>().SingleInstance();
        }
    }
}