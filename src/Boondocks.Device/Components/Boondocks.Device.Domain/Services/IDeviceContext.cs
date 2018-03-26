using System;

namespace Boondocks.Device.Domain.Services
{
    /// <summary>
    /// Referenced when a device token is validated to set the value of the 
    /// DeviceId contained within the token.  Application components inject
    /// this interface to obtain the DeviceId of the current request.   
    /// </summary>
    public interface IDeviceContext
    {

        /// <summary>
        /// The DeviceId contained within a valid JWT signed token.
        /// </summary>
        /// <returns>The identity value of the device.</returns>
        Guid DeviceId { get; }

        /// <summary>
        /// Invoked after the signed JWT token is validated to set
        /// the device identity contained within the token.
        /// </summary>
        /// <param name="deviceId">Identifies the device.</param>
        void SetRequestingDeviceId(Guid deviceId);
    }
}