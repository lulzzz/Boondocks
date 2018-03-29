using System;
using NetFusion.Rest.Client.Resources;

namespace Boondocks.Device.Client.Resource
{
    public class DeviceConfigResource : HalResource
    {
        public Guid DeviceId { get; set; }
        public string RegistryName {get; set; }
    }
}