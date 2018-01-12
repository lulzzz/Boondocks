using System;
using Autofac;
using Boondocks.Services.Contracts;
using Boondocks.Supervisor.Model;

namespace Boondocks.Supervisor
{
    internal static class ContainerFactory
    {
        public static IContainer Create()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new DeviceConfiguration
            {
                DeviceId = new Guid("7A6B6940-C7C1-482C-B382-0AC2FE2A96B0"),
                DeviceKey = new Guid("208F075A-59CE-486B-B98E-1436D62224EA"),
                DeviceApiUrl = "http://localhost:54983/",
                DockerEndpoint = "http://10.0.4.72:2375",
                PollSeconds = 10
            });

            builder.RegisterInstance(new DeviceStateProvider()
            {
                State = DeviceState.Idle
            });

            builder.RegisterType<UptimeProvider>().SingleInstance();
            builder.RegisterType<SupervisorHost>().SingleInstance();
            builder.RegisterType<ApplicationContainerFactory>().SingleInstance();
            builder.RegisterType<PathFactory>().SingleInstance();
            builder.RegisterType<OperationalStateProvider>().SingleInstance();

            return builder.Build();
        }
    }
}