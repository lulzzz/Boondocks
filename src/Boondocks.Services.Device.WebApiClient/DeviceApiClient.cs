namespace Boondocks.Services.Device.WebApiClient
{
    using System;
    using Endpoints;
    using Services.WebApiClient;

    public class DeviceApiClient
    {
        public DeviceApiClient(Guid deviceId, Guid deviceKey, string baseUri)
        {
            var client = new ApiClient(new Uri(baseUri));
            var tokenFactory = new TokenFactory(deviceId, deviceKey);

            ApplicationDownloadInfo = new ApplicationDownloadInfoOperations(client, tokenFactory);
            Configuration = new ConfigurationOperations(client, tokenFactory);
            Heartbeat = new HeartbeatOperations(client, tokenFactory);
            SupervisorDownloadInfo = new SupervisorDownloadInfoOperations(client, tokenFactory);
        }

        public ApplicationDownloadInfoOperations ApplicationDownloadInfo { get; }

        public ConfigurationOperations Configuration { get; }

        public HeartbeatOperations Heartbeat { get; }

        public SupervisorDownloadInfoOperations SupervisorDownloadInfo { get; }
    }
}