using System;
using System.Collections.Generic;

namespace Boondocks.Device.Api.Models
{
    public class MicroserviceHealthCheck
    {
        public IDictionary<string, DateTime?> DatabaseStatus { get; set; }
    }
}