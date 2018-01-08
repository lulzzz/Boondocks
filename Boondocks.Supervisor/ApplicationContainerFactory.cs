using System;
using Autofac;
using Boondocks.Services.Contracts;
using Boondocks.Supervisor.Model;

namespace Boondocks.Supervisor
{
    internal static class ApplicationContainerFactory
    {
        public static IContainer Create()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new DeviceConfiguration
            {
                DeviceId = new Guid("B77D8247-037D-4744-940D-53E312F5E4EF"),
                DeviceKey = new Guid("D0A974F0-2529-4143-88D8-E6F05C268FD9"),
                DeviceApiUrl = "http://localhost:54983/",
                DockerEndpoint = "http://10.0.4.72:2375",
                PollSeconds = 10
            });

            builder.RegisterInstance(new DeviceStateProvider()
            {
                State = DeviceState.Idle
            });

            builder.RegisterType<UptimeProvider>().SingleInstance();

            builder.RegisterType<SupervisorHost>();

            return builder.Build();
        }
    }
}