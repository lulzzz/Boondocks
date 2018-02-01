namespace Boondocks.Agent.Model
{
    using System;
    using System.IO;
    using Domain;
    using Interfaces;
    using Newtonsoft.Json;

    internal class DeviceConfigurationProvider : IDeviceConfigurationProvider
    {
        private readonly IDeviceConfigurationOverride _deviceConfigurationOverride;
        private readonly PathFactory _pathFactory;

        public DeviceConfigurationProvider(PathFactory pathFactory,
            IDeviceConfigurationOverride deviceConfigurationOverride)
        {
            _deviceConfigurationOverride = deviceConfigurationOverride;
            _pathFactory = pathFactory ?? throw new ArgumentNullException(nameof(pathFactory));
        }

        public IDeviceConfiguration GetDeviceConfiguration()
        {
            //Get the json
            var json = File.ReadAllText(_pathFactory.DeviceConfigFile);

            //Deserialize it
            var configuration = JsonConvert.DeserializeObject<DeviceConfiguration>(json);

            if (!string.IsNullOrWhiteSpace(_deviceConfigurationOverride.DeviceApiUrl))
            {
                configuration.DeviceApiUrl = _deviceConfigurationOverride.DeviceApiUrl;
                Console.WriteLine("Overring DeviceApiUrl.");
            }

            if (_deviceConfigurationOverride.DeviceId != null)
            {
                configuration.DeviceId = _deviceConfigurationOverride.DeviceId.Value;
                Console.WriteLine("Overring DeviceId.");
            }

            if (_deviceConfigurationOverride.DeviceKey != null)
            {
                configuration.DeviceKey = _deviceConfigurationOverride.DeviceKey.Value;
                Console.WriteLine("Overring DeviceKey.");
            }

            if (!string.IsNullOrWhiteSpace(_deviceConfigurationOverride.DockerEndpoint))
            {
                configuration.DockerEndpoint = _deviceConfigurationOverride.DockerEndpoint;
                Console.WriteLine("Overring DockerEndpoint.");
            }

            if (_deviceConfigurationOverride.PollSeconds != null)
            {
                configuration.PollSeconds = _deviceConfigurationOverride.PollSeconds.Value;
                Console.WriteLine("Overring PollSeconds.");
            }

            return configuration;
        }
    }
}