using System;
using Boondocks.Services.Device.WebApiClient.Endpoints;
using Boondocks.Services.WebApiClient;

namespace Boondocks.Services.Device.WebApiClient
{
    public class DeviceApiClient
    {
        public DeviceApiClient(Guid deviceId, Guid deviceKey, string baseUri)
        {
            var client = new ApiClient(new Uri(baseUri));
            var tokenFactory = new TokenFactory(deviceId, deviceKey);

            Configuration = new ConfigurationOperations(client, tokenFactory);
            Heartbeat = new HeartbeatOperations(client, tokenFactory);
        }

        public ConfigurationOperations Configuration { get; }

        public HeartbeatOperations Heartbeat { get; }
    }
}