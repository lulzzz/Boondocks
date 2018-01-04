using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateDeviceResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("device-key")]
        public Guid DeviceKey { get; set; }
    }

    public class GetDevicesResponse
    {
        public Device[] Devices { get; set; }

        public class Device
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public DateTime CreatedUtc { get; set; }
        }
    }

    


}