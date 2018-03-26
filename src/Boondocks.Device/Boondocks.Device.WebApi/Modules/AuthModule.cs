using Autofac;
using Boondocks.Auth.Infra.Repositories;
using Boondocks.Base.Auth;
using Boondocks.Base.Auth.Core;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Device.WebApi.Modules
{
    public class AuthModule : PluginModule
    {
        public override void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<DeviceKeyAuthRepository>()
                .As<IDeviceKeyAuthRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DeviceAuthService>()
                .As<IDeviceAuthService>()
                .InstancePerLifetimeScope();

        }
    }
}
