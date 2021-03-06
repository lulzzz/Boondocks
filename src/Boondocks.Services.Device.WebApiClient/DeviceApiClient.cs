﻿namespace Boondocks.Services.Device.WebApiClient
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
            ApplicationLogs = new ApplicationLogOperations(client, tokenFactory);
            Configuration = new ConfigurationOperations(client, tokenFactory);
            Heartbeat = new HeartbeatOperations(client, tokenFactory);
            AgentDownloadInfo = new AgentDownloadInfoOperations(client, tokenFactory);
        }

        public ApplicationDownloadInfoOperations ApplicationDownloadInfo { get; }

        public ApplicationLogOperations ApplicationLogs { get; }

        public ConfigurationOperations Configuration { get; }

        public HeartbeatOperations Heartbeat { get; }

        public AgentDownloadInfoOperations AgentDownloadInfo { get; }
    }
}