using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetFusion.Bootstrap.Configuration;

namespace Boondocks.Device.WebApi
{
    // Setup configuration and logging then invokes the application 
    // Startup class which configures the HTTP Request pipeline and
    // the NetFusion application-container. 
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configBuilder) => SetupConfiguration(context, args, configBuilder))
                .ConfigureLogging(SetupLogging)                
                .UseStartup<Startup>()
                .Build();

        // Configure application configuration providers.  When running within Docker,
        // the configuration settings will be read from environment variables.
        private static void SetupConfiguration(
            WebHostBuilderContext context, 
            string[] args,
            IConfigurationBuilder configBuilder)
        {
            // Settings contained within the appsettings.json file are only to be used
            // as a developer convenience.  In other environments, the settings are to 
            // be specified using docker environment variables.
            if (EnvironmentConfig.IsDevelopment)
            {
                configBuilder.AddDefaultAppSettings();
            }
 
            configBuilder.AddEnvironmentVariables();
            configBuilder.AddCommandLine(args);
        }

        // Configure logging for the service.  TODO:  Add additional logger once we
        // have decided which one will be used.
        private static void SetupLogging(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.AddDebug().SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Trace);
        }
    }
}
