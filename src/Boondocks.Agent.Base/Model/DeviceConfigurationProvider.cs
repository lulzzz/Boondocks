namespace Boondocks.Agent.Base.Model
{
    using System;
    using System.IO;
    using Interfaces;
    using Newtonsoft.Json;
    using Services.Contracts;
    using Services.Contracts.Interfaces;

    public class DeviceConfigurationProvider : IDeviceConfigurationProvider
    {
        private readonly IPathFactory _pathFactory;

        public DeviceConfigurationProvider(IPathFactory pathFactory)
        {
            _pathFactory = pathFactory ?? throw new ArgumentNullException(nameof(pathFactory));
        }

        public bool Exists()
        {
            Console.WriteLine($"Looking for device configuration at '{_pathFactory.DeviceConfigFile}'...");

            return File.Exists(_pathFactory.DeviceConfigFile);
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