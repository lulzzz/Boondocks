using System;
using System.Collections.Generic;

namespace Boondocks.Device.Domain
{
    /// <summary>
    /// Domain entity containing information that can be used to
    /// determine if the microservice is correctly functioning.
    /// </summary>
    public class ServiceStatus
    {
       public IDictionary<string, DateTime?> LastDataUpdates { get; }

       public ServiceStatus(IDictionary<string, DateTime?> updates)
       {
           LastDataUpdates = updates;
       }
    }
}