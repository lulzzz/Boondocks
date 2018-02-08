namespace Boondocks.Agent.Model
{
    using System;
    using System.IO;
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

            Console.WriteLine("Overriding the device api uri...");

            //Do a horribly hacky override here because I don't have physical access to my pi right now
            configuration.DeviceApiUrl = "http://desktop-richq.captiveaire.com/Boondocks.Services.Device.WebApi/";

            return configuration;
        }
    }
}