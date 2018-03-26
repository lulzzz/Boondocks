using Autofac;
using Boondocks.Base.Auth.Core;
using Microsoft.AspNetCore.Http;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Base.Auth.Modules
{
    public class AuthModule : PluginModule
    {
        public override void RegisterDefaultComponents(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            builder.RegisterType<DeviceKeyAuthRepository>()
                .As<IDeviceKeyAuthRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DeviceAuthService>()
                .As<IDeviceAuthService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DeviceContext>()
                .As<IDeviceContext>()
                .InstancePerLifetimeScope();
        }
    }
}
