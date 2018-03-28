using System;
using System.Security.Principal;

namespace Boondocks.Base.Auth
{
    /// <summary>
    /// Identity used to associate the authenticated device with
    /// the current thread.
    /// </summary>
    public class DeviceIdentity : IIdentity
    {
        public string AuthenticationType => "Device";
        
        public DeviceIdentity(Guid deviceId, DeviceAuthResult result)
        {
            IsAuthenticated = result.IsAuthenticated;
            Name = deviceId.ToString("D");
        }

        public bool IsAuthenticated { get; }

        public string Name { get; }
    }
}
