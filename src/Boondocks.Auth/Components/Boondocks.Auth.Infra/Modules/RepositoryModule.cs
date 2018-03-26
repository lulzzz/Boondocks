using Autofac;
using Boondocks.Auth.Infra.Repositories;
using Boondocks.Base.Auth;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Auth.Modules
{
    // Convention based registration.
    public class RepositoryModule : PluginModule
    {
        public override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DeviceKeyAuthRepository>()
                .As<IDeviceKeyAuthRepository>()
                .InstancePerLifetimeScope();
        }

        public override void ScanPlugin(TypeRegistration registration)
        {
            registration.PluginTypes.Where(t => t.Name.EndsWith("Repository", System.StringComparison.Ordinal))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}