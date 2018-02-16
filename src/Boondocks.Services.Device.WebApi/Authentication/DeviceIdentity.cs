namespace Boondocks.Services.Device.WebApi.Authentication
{
    using System;
    using System.Security.Principal;

    public class DeviceIdentity : IIdentity
    {
        public DeviceIdentity(Guid deviceId, bool isAuthenticated)
            : this(deviceId.ToString("D"), isAuthenticated)
        {
        }

        public DeviceIdentity(string deviceId, bool isAuthenticated) : this(isAuthenticated)
        {
            Name = deviceId;
        }

        private DeviceIdentity(bool isAuthenticated)
        {
            IsAuthenticated = isAuthenticated;
        }

        public string AuthenticationType => "Device";

        public bool IsAuthenticated { get; }

        public string Name { get; }
    }
}