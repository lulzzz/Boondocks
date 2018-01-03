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
                DeviceId = "5781DBE685A742CCBCC1A5AF9E88EA65",
                DeviceKey = "F9FD8B0754AA467D9F1B8F5695EA8800",
                DeviceApiUrl = "http://localhost:54983/",
                DockerEndpoint = "http://10.0.4.72:2375",
                PollSeconds = 10
            });

            builder.RegisterType<SupervisorHost>();

            return builder.Build();
        }
    }
}