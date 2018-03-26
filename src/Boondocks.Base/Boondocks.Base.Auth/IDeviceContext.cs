using System;

namespace Boondocks.Base.Auth
{
    public interface IDeviceContext
    {
        Guid DeviceId { get; }
    }
}
