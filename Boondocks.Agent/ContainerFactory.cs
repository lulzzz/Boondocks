using Autofac;
using Boondocks.Agent.Interfaces;
using Boondocks.Agent.Model;

namespace Boondocks.Agent
{
    internal static class ContainerFactory
    {
        public static IContainer Create()
        {
            var builder = new ContainerBuilder();

            builder.Register<IDeviceConfiguration>(context =>
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