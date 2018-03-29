using System;
using NetFusion.Rest.Resources.Hal;

namespace Boondocks.Device.Api.Resources
{
    public class DeviceConfigResource : HalResource
    {
        public Guid DeviceId { get; set; }
        public string RegistryName {get; set; }
        
    }
}