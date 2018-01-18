using System;
using System.IO;
using Boondocks.Agent.Interfaces;
using Newtonsoft.Json;

namespace Boondocks.Agent.Model
{
    public class DeviceConfigurationProvider : IDeviceConfigurationProvider
    {
        private readonly PathFactory _pathFactory;

        public DeviceConfigurationProvider(PathFactory pathFactory)
        {
            _pathFactory = pathFactory ?? throw new ArgumentNullException(nameof(pathFactory));
        }

        public IDeviceConfiguration GetDeviceConfiguration()
        {
            //Get the json
            string json = File.ReadAllText(_pathFactory.DeviceConfigFile);

            //Deserialize it
            return JsonConvert.DeserializeObject<DeviceConfiguration>(json);
        }
    }
}