using System;
using System.Security.Principal;

namespace Boondocks.Services.Device.WebApi.Authentication
{
    public class DeviceIdentity : IIdentity
    {
        public DeviceIdentity(Guid deviceId, bool isAuthenticated)
        {
            DeviceId = deviceId;
            IsAuthenticated = isAuthenticated;
        }

        public string AuthenticationType => "Device";

        public Guid DeviceId { get; }

        public bool IsAuthenticated { get; }

        public string Name => DeviceId.ToString("N");
    }
}