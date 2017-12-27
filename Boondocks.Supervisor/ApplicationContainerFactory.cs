using Autofac;

namespace Boondocks.Supervisor
{
    internal static class ApplicationContainerFactory
    {
        public static IContainer Create()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SupervisorHost>();

            return builder.Build();
        }
    }
}