using System;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Contracts;
using Boondocks.Services.Management.Contracts;

namespace Boondocks.Services.Management.WebApiClient
{
    public class ManagementApiClient : CaptiveAire.WebApiClient.WebApiClient
    {
        private static class ResourceUrls
        {
            public const string Applications = "v1/applications";
            public const string Devices = "v1/devices";
            public const string ApplicationVersions = "v1/applicationVersions";
            public const string DeviceTypes = "v1/deviceTypes";
        }

        public ManagementApiClient(string baseUri)
        {
            BaseUri = baseUri;
        }

        protected override string BaseUri { get; }

        public Task<DeviceType[]> GetDeviceTypes(CancellationToken cancellationToken = new CancellationToken())
        {
            return GetAsync<DeviceType[]>(ResourceUrls.DeviceTypes, null, cancellationToken);
        }

        public Task<Application> CreateApplicationAsync(Guid deviceTypeId, string name, CancellationToken cancellationToken = new CancellationToken())
        {
            var request = new CreateApplicationRequest
            {
                DeviceTypeId = deviceTypeId,
                Name = name
            };

            return PostAsync<Application>(ResourceUrls.Applications, request, null, cancellationToken);
        }

        public Task<Application[]> GetApplicationsAsync(GetApplicationsRequest request = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (request == null)
                request = new GetApplicationsRequest();

            dynamic query = new ExpandoObject();

            if (request.DeviceTypeId != null)
                query.deviceTypeId = request.DeviceTypeId.Value;

            return GetAsync<Application[]>(ResourceUrls.Applications, query, cancellationToken);
        }

        public Task<ApplicationVersion> UploadApplicationVersionAsync(Guid applicationId, string name, Stream stream, Guid? versionId = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var queryValues = new
            {
                id = versionId,
                name,
                applicationId
            };

            return UploadFileAsync<ApplicationVersion>(
                ResourceUrls.ApplicationVersions, 
                "image.image", 
                stream, 
                queryValues, 
                cancellationToken);
        }

        public Task<Device> CreateDeviceAsync(Guid applicationId, string name, Guid? applicationVersionId = null, Guid? deviceKey = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var request = new CreateDeviceRequest()
            {
                ApplicationId = applicationId,
                Name = name,
                DeviceKey = deviceKey
            };

            return PostAsync<Device>(ResourceUrls.Devices, request, null, cancellationToken);
        }

        public Task<Device[]> GetDevicesAsync(GetDevicesRequest request = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (request == null)
                request = new GetDevicesRequest();

            dynamic query = new ExpandoObject();

            if (request.ApplicationId != null)
            {
                query.applicationId = request.ApplicationId.Value;
            }

            return GetAsync<Device[]>(ResourceUrls.Devices, query, cancellationToken);
        }
    }

    public class GetApplicationsRequest
    {
        public Guid? DeviceTypeId { get; set; }
    }

    public class GetDevicesRequest
    {
        public Guid? ApplicationId { get; set; }
    }
}
