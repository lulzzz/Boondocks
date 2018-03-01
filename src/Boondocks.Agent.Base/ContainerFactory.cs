namespace Boondocks.Agent.Base
{
    using System;
    using Autofac;
    using Model;
    using Services.Contracts.Interfaces;
    using Services.Device.WebApiClient;

    public static partial class ContainerFactory
    {
        public static IContainer Create(PathFactory pathFactory, IDeviceConfiguration deviceConfiguration)
        {
            if (pathFactory == null) throw new ArgumentNullException(nameof(pathFactory));
            if (deviceConfiguration == null) throw new ArgumentNullException(nameof(deviceConfiguration));

            var builder = new ContainerBuilder();

            //Device api
            builder.Register(context => new DeviceApiClient(
                deviceConfiguration.DeviceId,
                deviceConfiguration.DeviceKey,
                deviceConfiguration.DeviceApiUrl)).SingleInstance();


            builder.RegisterInstance(pathFactory);
            builder.RegisterInstance(deviceConfiguration);

            builder.RegisterModule<AgentModule>();

            return builder.Build();
        }
    }
}