﻿namespace Boondocks.Services.Management.WebApiClient
{
    using System;
    using Endpoints;
    using Services.WebApiClient;

    public class ManagementApiClient
    {
        public ManagementApiClient(string baseUri, TimeSpan? defaultTimeout = null)
        {
            var client = new ApiClient(new Uri(baseUri), defaultTimeout);

            AgentVersions = new AgentVersionOperations(client);
            AgentUploadInfo = new AgentUploadInfoOperations(client);
            Applications = new ApplicationOperations(client);
            ApplicationLogs = new ApplicationLogOperations(client);
            ApplicationEnvironmentVariables = new ApplicationEnvironmentVariableOperations(client);
            ApplicationVersions = new ApplicationVersionOperations(client);
            ApplicationUpload = new ApplicationUploadInfoOperations(client);
            Devices = new DeviceOperations(client);
            DeviceConfiguration = new DeviceConfigurationOperations(client);
            DeviceEnvironmentVariables = new DeviceEnvironmentVariableOperations(client);
            DeviceTypes = new DeviceTypeOperations(client);
            Health = new HealthOperations(client);
        }

        public AgentVersionOperations AgentVersions { get; }

        public AgentUploadInfoOperations AgentUploadInfo { get; }

        public ApplicationOperations Applications { get; }

        public ApplicationLogOperations ApplicationLogs { get; }

        public ApplicationEnvironmentVariableOperations ApplicationEnvironmentVariables { get; }

        public ApplicationVersionOperations ApplicationVersions { get; }

        public ApplicationUploadInfoOperations ApplicationUpload { get; }

        public DeviceOperations Devices { get; }

        public DeviceConfigurationOperations DeviceConfiguration { get; }

        public DeviceEnvironmentVariableOperations DeviceEnvironmentVariables { get; }

        public DeviceTypeOperations DeviceTypes { get; }

        public HealthOperations Health { get; } 
    }

    public class GetApplicationVersionsRequest
    {
        public Guid? ApplicationId { get; set; }
    }
}