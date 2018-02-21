namespace Boondocks.Agent
{
    using System;
    using Autofac;
    using Docker.DotNet;
    using Domain;
    using Interfaces;
    using Model;
    using Serilog;
    using Services.Contracts.Interfaces;
    using Services.Device.WebApiClient;
    using Update;

    internal static class ContainerFactory
    {
        public static IContainer Create(PathFactory pathFactory, IDeviceConfiguration deviceConfiguration)
        {
            if (pathFactory == null) throw new ArgumentNullException(nameof(pathFactory));
            if (deviceConfiguration == null) throw new ArgumentNullException(nameof(deviceConfiguration));

            var builder = new ContainerBuilder();

            builder.Register(context =>
            {
                var provider = context.Resolve<IDeviceConfigurationProvider>();

                return provider.GetDeviceConfiguration();
            }).SingleInstance();

            builder.RegisterInstance(pathFactory);
            builder.RegisterInstance(deviceConfiguration);

            builder.RegisterType<DeviceStateProvider>().SingleInstance();
            builder.RegisterType<UptimeProvider>().As<IUptimeProvider>().SingleInstance();
            builder.RegisterType<AgentHost>().As<IAgentHost>().SingleInstance();
            builder.RegisterType<ApplicationDockerContainerFactory>().SingleInstance();
            builder.RegisterType<AgentDockerContainerFactory>().SingleInstance();
            
            builder.RegisterType<PlatformDetector>().As<IPlatformDetector>().SingleInstance();
            builder.RegisterType<EnvironmentConfigurationProvider>().As<IEnvironmentConfigurationProvider>().SingleInstance();

            //Update services
            builder.RegisterType<ApplicationUpdateService>().SingleInstance();
            builder.RegisterType<AgentUpdateService>().SingleInstance();

            //Device api
            builder.Register(context => new DeviceApiClient(
                deviceConfiguration.DeviceId,
                deviceConfiguration.DeviceKey,
                deviceConfiguration.DeviceApiUrl)).SingleInstance();

            //IDockerClient
            builder.Register(context =>
            {
                IEnvironmentConfigurationProvider environmentConfigurationProvider = context.Resolve<IEnvironmentConfigurationProvider>();

                var dockerClientConfiguration = new DockerClientConfiguration(new Uri(environmentConfigurationProvider.DockerEndpoint));

                return dockerClientConfiguration.CreateClient();
               
            }).As<IDockerClient>().SingleInstance();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            builder.RegisterInstance(Log.Logger);

            return builder.Build();
        }
    }
}