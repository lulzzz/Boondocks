using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetFusion.Bootstrap.Configuration;
using NetFusion.Bootstrap.Container;
using NetFusion.Bootstrap.Plugins;
using NetFusion.Web.Mvc;

namespace Boondocks.Device.WebApi.Bootstrap
{
    public class AppContainerSetup
    {
        public static void Bootstrap(
            IConfiguration configuration,
            ILoggerFactory loggerFactory, 
            IServiceCollection services)
        {
            // Creates an instance of a type resolver that will look for plug-ins within 
            // the assemblies matching the passed patterns.  The implementation delegates
            // to the .NET core classes used to discover/probe for assemblies.
            var typeResolver = new TypeResolver(
                "Boondocks.Device.WebApi", 
                "Boondocks.Device.*",
                "Boondocks.Base.*"); 

            // The following configures the NetFusion AppContainer. 
            AppContainer.Create(typeResolver)
                .WithConfig(configInit: (EnvironmentConfig envConfig) => {

                    // Tell NetFusion to use this configuration setup.
                    envConfig.UseConfiguration(configuration);
                })

                .WithConfig((LoggerConfig config) => {

                    // Tell NetFusion to use this logging setup.
                    config.UseLoggerFactory(loggerFactory);

                    // If there is an exception, the AppContainer will log it.
                    // If false, it is the responsibility of the bootstrap code to log the exception.
                    config.LogExceptions = true; 
                })  
        
                .WithConfig((AutofacRegistrationConfig config) => {
                    config.Build = builder =>
                    {
                      
                        builder.RegisterInstance(new SqlServerDbConnectionFactory())
                            .As<IDbConnectionFactory>();
                    
                        // This method is Autofac and it is registering any services contained
                        // with the services collection added by ASP.NET (i.e. controllers).
                        builder.Populate(services);
                    };
                })

                .WithConfig((WebMvcConfig config) =>
                {
                    config.EnableRouteMetadata = true;
                    config.UseServices(services);
                })
                .Build()    // Discover all plug-ins and build the DI Container.
                .Start();   // Visit each plug-in and allow it to execute any code needed at start of application.
        }
    }
}
