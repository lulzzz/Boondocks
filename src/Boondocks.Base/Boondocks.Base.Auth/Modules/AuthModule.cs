﻿using Autofac;
using Boondocks.Base.Auth.Core;
using Microsoft.AspNetCore.Http;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Base.Auth.Modules
{
    /// <summary>
    /// Registers services provided by the plugin as default implementations.
    /// </summary>
    public class AuthModule : PluginModule
    {
        public override void RegisterDefaultComponents(ContainerBuilder builder)
        {
            // Allows access to the current HTTP Context
            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            // Register repository responsible for querying DeviceKeys.
            builder.RegisterType<DeviceKeyAuthRepository>()
                .As<IDeviceKeyAuthRepository>()
                .InstancePerLifetimeScope();

            // Service responsible for determining if a signed device-key is valid.
            builder.RegisterType<DeviceAuthService>()
                .As<IDeviceAuthService>()
                .InstancePerLifetimeScope();

            // Allows code to access the DeviceId associated the authenticated request.
            builder.RegisterType<DeviceContext>()
                .As<IDeviceContext>()
                .InstancePerLifetimeScope();               
        }
    }
}
