namespace Boondocks.Agent.Base
{
    using System;
    using AgentLogging;
    using Autofac;
    using Model;
    using Services.Contracts.Interfaces;
    using Services.Device.WebApiClient;

    public static partial class ContainerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathFactory"></param>
        /// <param name="deviceConfiguration"></param>
        /// <param name="customModule">Must provide IRootFileSysteUpdateService.</param>
        /// <returns></returns>
        public static IContainer Create(IPathFactory pathFactory, IDeviceConfiguration deviceConfiguration, Module customModule = null)
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

            if (customModule != null)
            {
                builder.RegisterModule(customModule);
            }

            builder.RegisterModule<AgentModule>();

            var container = builder.Build();

            //TODO: Find less horribly hacky way to do this.
            var deviceApiClient = container.Resolve<DeviceApiClient>();
            var sink = container.Resolve<AgentLogSink>();
            sink.DeviceApiClient = deviceApiClient;

            return container;
        }
    }
}