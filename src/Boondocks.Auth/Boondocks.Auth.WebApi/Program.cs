using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetFusion.Bootstrap.Configuration;

namespace Boondocks.Auth.WebApi
{
    // Initializes the application's configuration and logging then delegates 
    // to the Startup class to initialize HTTP pipeline related settings.
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

        private static void SetupConfiguration(
            WebHostBuilderContext context, 
            string[] args,
            IConfigurationBuilder configBuilder)
        {
            // Settings contained within the appsettings.json file are only to be used
            // as a developer convenience.  In other environments, the settings are to 
            // be specified using environment variables.
            if (EnvironmentConfig.IsDevelopment)
            {
                configBuilder.AddDefaultAppSettings();
            }
 
            configBuilder.AddEnvironmentVariables();
            configBuilder.AddCommandLine(args);
        }

        private static void SetupLogging(WebHostBuilderContext context, ILoggingBuilder loggingBuilder)
        {          
            var minLogLevel = GetMinLogLevel(context); 

            loggingBuilder.ClearProviders()
                .SetMinimumLevel(minLogLevel)
                .AddDebug().SetMinimumLevel(minLogLevel)
                .AddConsole().SetMinimumLevel(minLogLevel);
        }

        // Determines the minimum log level that should be used.  First a configuration value used to specify the 
        // minimum log level is checked.  If present, it will be used.  If not found, the minimum log level based 
        // on the application's execution environment is used.
        private static LogLevel GetMinLogLevel(WebHostBuilderContext context)
        {
            return context.Configuration.GetValue<LogLevel?>("Logging:MinLogLevel")
                ?? EnvironmentMinLogLevel;
        }

        private static LogLevel EnvironmentMinLogLevel =>
            EnvironmentConfig.IsDevelopment ? LogLevel.Trace
                : EnvironmentConfig.IsTest || EnvironmentConfig.IsStaging ? LogLevel.Debug
                : EnvironmentConfig.IsProduction ? LogLevel.Warning
                : LogLevel.Information;               
    }
}
