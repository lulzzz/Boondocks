using Boondocks.Device.Domain;
using NetFusion.Messaging.Types;

namespace Boondocks.Device.Api.Queries
{
    public class GetHealthCheckStatus : Query<ServiceStatus>
    {
        private GetHealthCheckStatus() {}

        public static GetHealthCheckStatus Query => new GetHealthCheckStatus();
    }
}