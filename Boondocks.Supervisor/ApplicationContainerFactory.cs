using System;
using Autofac;
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
                DeviceId = new Guid("133F0274-204B-4635-A232-0D0CEEF87730"),
                DeviceKey = new Guid("05438596-8975-4946-8304-00D76B532C6A"),
                DeviceApiUrl = "http://localhost:54983/",
                DockerEndpoint = "http://10.0.4.72:2375",
                PollSeconds = 10
            });

            builder.RegisterType<SupervisorHost>();

            return builder.Build();
        }
    }
}