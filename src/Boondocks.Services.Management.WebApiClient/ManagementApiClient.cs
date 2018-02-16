namespace Boondocks.Services.Management.WebApiClient
{
    using System;
    using Endpoints;
    using Services.WebApiClient;

    public class ManagementApiClient
    {
        public ManagementApiClient(string baseUri, TimeSpan? defaultTimeout = null)
        {
            var client = new ApiClient(new Uri(baseUri), defaultTimeout);

            Applications = new ApplicationOperations(client);
            ApplicationVersions = new ApplicationVersionOperations(client);
            ApplicationUpload = new ApplicationUploadInfoOperations(client);
            Devices = new DeviceOperations(client);
            DeviceConfiguration = new DeviceConfigurationOperations(client);
            DeviceTypes = new DeviceTypeOperations(client);
            DeviceArchitectures = new DeviceArchitectureOperations(client);
            SupervisorVersions = new SupervisorVersionOperations(client);
            SupervisorUploadInfo = new SupervisorUploadInfoOperations(client);
        }

        public ApplicationOperations Applications { get; }

        public ApplicationVersionOperations ApplicationVersions { get; }

        public ApplicationUploadInfoOperations ApplicationUpload { get; }

        public DeviceOperations Devices { get; }

        public DeviceConfigurationOperations DeviceConfiguration { get; }

        public DeviceTypeOperations DeviceTypes { get; }

        public DeviceArchitectureOperations DeviceArchitectures { get; }

        public SupervisorVersionOperations SupervisorVersions { get; }

        public SupervisorUploadInfoOperations SupervisorUploadInfo { get; }
    }

    public class GetApplicationVersionsRequest
    {
        public Guid? ApplicationId { get; set; }
    }
}