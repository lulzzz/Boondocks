using Boondocks.Device.Domain;
using NetFusion.Messaging.Types;

namespace Boondocks.Device.Api.Queries
{
    public class HealthCheckStatus : Query<ServiceStatus>
    {
        private HealthCheckStatus() {}

        public static HealthCheckStatus Query => new HealthCheckStatus();
    }
}