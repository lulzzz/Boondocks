using System;
using System.Threading.Tasks;
using Boondocks.Device.Client.Resource;
using NetFusion.Rest.Client;

namespace Boondocks.Device.Client
{
    public static class RequestClientExtensions
    {
        public static Task<DeviceConfigResource> GetConfiguration(this IRequestClient requestClient, string deviceToken)
        {
            throw new NotImplementedException();
        }

        public static Task<AppVersionResource> GetAppVersion(this IRequestClient requestClient, Guid id, string deviceToken)
        {
            throw new NotImplementedException();
        }

        public static Task<AgentVersionResource> GetAgentVersion(this IRequestClient requestClient, Guid id, string deviceToken)
        {
            throw new NotImplementedException();
        }
    }
}