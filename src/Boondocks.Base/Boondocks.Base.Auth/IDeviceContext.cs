using System;

namespace Boondocks.Base.Auth
{
    /// <summary>
    /// Service that can be injected into a component to reference the 
    /// DeviceId of an authenticated request.
    /// </summary>
    public interface IDeviceContext
    {
        /// <summary>
        /// The DeviceId of the authenticated request.  If not authenticated,
        /// Guid.Empty value is returned.
        /// </summary>
        Guid DeviceId { get; }
    }
}
