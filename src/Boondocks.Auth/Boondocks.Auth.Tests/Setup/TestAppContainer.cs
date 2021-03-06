using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetFusion.Bootstrap.Configuration;
using NetFusion.Bootstrap.Container;
using NetFusion.Test.Plugins;
using NetFusion.Web.Mvc;

namespace Boondocks.Auth.Tests.Setup
{
    /// <summary>
    /// Creates and configures the NetFusion AppContainer used by the unit-tests.
    /// </summary>
    public static class TestAppContainer
    {
        /// <summary>
        /// Creates an application container for use specifically for unit-tests.
        /// </summary>
        /// <param name="hostPlugin">The plugin-representing the host application.</param>
        /// <param name="services">And services to add to the application's container.</param>
        /// <returns>Created application container.</returns>
        public static IAppContainer Create(MockAppHostPlugin hostPlugin, IServiceCollection services)
        {
            var resolver = new TestTypeResolver();
           
            resolver.AddPlugin(hostPlugin);
            var container = new AppContainer(resolver, setGlobalReference: false);

            container.WithConfig((EnvironmentConfig envConfig) =>
            {
                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddDefaultAppSettings();

                envConfig.UseConfiguration(configBuilder.Build());
            })
         
            .WithConfig((AutofacRegistrationConfig config) =>
            {
                config.Build = builder =>
                {
                    builder.Populate(services);
                };
            })

            .WithConfig((WebMvcConfig config) =>
            {
                config.EnableRouteMetadata = true;
                config.UseServices(services);
            });

            return container;
        }
    }
}