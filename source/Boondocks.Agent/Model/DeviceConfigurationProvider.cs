namespace Boondocks.Agent.Model
{
    using System;
    using System.IO;
    using Domain;
    using Interfaces;
    using Newtonsoft.Json;
    using Services.Contracts;
    using Services.Contracts.Interfaces;

    internal class DeviceConfigurationProvider : IDeviceConfigurationProvider
    {
        private readonly PathFactory _pathFactory;

        public DeviceConfigurationProvider(PathFactory pathFactory)
        {
            _pathFactory = pathFactory ?? throw new ArgumentNullException(nameof(pathFactory));
        }

        public IDeviceConfiguration GetDeviceConfiguration()
        {
            //Get the json
            var json = File.ReadAllText(_pathFactory.DeviceConfigFile);

            //Deserialize it
            var configuration = JsonConvert.DeserializeObject<DeviceConfiguration>(json);


            return configuration;
        }
    }
}