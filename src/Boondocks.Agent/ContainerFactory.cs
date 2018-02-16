namespace Boondocks.Agent
{
    using Autofac;
    using Domain;
    using Interfaces;
    using Model;
    using Serilog;

    internal static class ContainerFactory
    {
        public static IContainer Create()
        {
            var builder = new ContainerBuilder();

            builder.Register(context =>
            {
                var provider = context.Resolve<IDeviceConfigurationProvider>();

                return provider.GetDeviceConfiguration();
            }).SingleInstance();

            builder.RegisterType<DeviceStateProvider>().SingleInstance();
            builder.RegisterType<DeviceConfigurationProvider>().As<IDeviceConfigurationProvider>();
            builder.RegisterType<UptimeProvider>().As<IUptimeProvider>().SingleInstance();
            builder.RegisterType<AgentHost>().As<IAgentHost>().SingleInstance();
            builder.RegisterType<ApplicationDockerContainerFactory>().SingleInstance();
            builder.RegisterType<AgentDockerContainerFactory>().SingleInstance();
            builder.RegisterType<PathFactory>().SingleInstance();
            builder.RegisterType<OperationalStateProvider>().SingleInstance();
            builder.RegisterType<PlatformDetector>().As<IPlatformDetector>().SingleInstance();
            builder.RegisterType<EnvironmentConfigurationProvider>().As<IEnvironmentConfigurationProvider>().SingleInstance();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            builder.RegisterInstance(Log.Logger);

            return builder.Build();
        }
    }
}