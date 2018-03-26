using System;
using Boondocks.Device.Domain.Services;

namespace Boondocks.Device.App.Services
{
    /// <summary>
    /// Contains information about the device associated with the current request.
    /// </summary>
    public class DeviceContext : IDeviceContext
    {
        private Guid _deviceId = Guid.Empty;

        public Guid DeviceId 
        { 
            get 
            {
                if (_deviceId == Guid.Empty) 
                {
                    throw new InvalidOperationException(
                        "The DeviceId associated with the current request has not been set.");
                } 
                return _deviceId; 
            }
        }

        public void SetRequestingDeviceId(Guid deviceId)
        {
            _deviceId = deviceId; 
        }
    }
}