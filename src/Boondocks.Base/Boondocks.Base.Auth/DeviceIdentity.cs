using System;
using System.Security.Principal;

namespace Boondocks.Base.Auth
{
    public class DeviceIdentity : IIdentity
    {
        public string AuthenticationType => "Device";
        
        public DeviceIdentity(Guid deviceId, KeyAuthResult result)
        {
            IsAuthenticated = result.IsAuthenticated;
            Name = deviceId.ToString("D");
        }

        public bool IsAuthenticated { get; }

        public string Name { get; }
    }
}
