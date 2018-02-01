namespace Boondocks.Agent
{
    using Autofac;
    using Interfaces;
    using Model;

    internal static class ContainerFactory
    {
        public static IContainer Create(IDeviceConfigurationOverride deviceConfigurationOverride)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(deviceConfigurationOverride);

            builder.Register(context =>
            {
                var provider = context.Resolve<IDeviceConfigurationProvider>();

                return provider.GetDeviceConfiguration();
            });

            builder.RegisterType<DeviceStateProvider>().SingleInstance();
            builder.RegisterType<DeviceConfigurationProvider>().As<IDeviceConfigurationProvider>();
            builder.RegisterType<UptimeProvider>().As<IUptimeProvider>().SingleInstance();
            builder.RegisterType<AgentHost>().As<IAgentHost>().SingleInstance();
            builder.RegisterType<ApplicationDockerContainerFactory>().SingleInstance();
            builder.RegisterType<PathFactory>().SingleInstance();
            builder.RegisterType<OperationalStateProvider>().SingleInstance();
            builder.RegisterType<PlatformDetector>().As<IPlatformDetector>().SingleInstance();

            return builder.Build();
        }
    }
}