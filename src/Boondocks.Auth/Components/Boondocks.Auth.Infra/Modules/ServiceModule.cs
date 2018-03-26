using Autofac;
using Boondocks.Base.Auth;
using Boondocks.Base.Auth.Core;
using NetFusion.Bootstrap.Plugins;
using System;

namespace Boondocks.Auth.Infra.Modules
{
    /// <summary>
    /// Adds all services to the dependency-injection container by convention.
    /// </summary>
    public class ServiceModule : PluginModule
    {
        public override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DeviceAuthService>()
                .As<IDeviceAuthService>()
                .InstancePerLifetimeScope();
        }

        public override void ScanPlugin(TypeRegistration registration)
       {
            registration.PluginTypes.Where(t => t.Name.EndsWith("Service", StringComparison.Ordinal))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
       }
    }
}