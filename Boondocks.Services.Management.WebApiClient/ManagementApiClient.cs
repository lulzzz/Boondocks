using System;
using System.Dynamic;
using System.IO;
using System.Net.Http;
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

        public Task<Application> GetApplicationAsync(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetAsync<Application>(ResourceUrls.Applications, new {id}, cancellationToken);
        }

        public Task UpdateApplicationAsync(Application application, CancellationToken cancellationToken = new CancellationToken())
        {
            return PutAsync(ResourceUrls.Applications, application, null, cancellationToken);
        }

        public Task<ApplicationVersion> UploadApplicationVersionAsync(Guid applicationId, string name, Stream stream, Guid? versionId = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var queryValues = new
            {
                id = versionId,
                name,
                applicationId
            };

            return UploadFileAsyncLocal<ApplicationVersion>(
                ResourceUrls.ApplicationVersions, 
                "image.image", 
                stream, 
                queryValues, 
                "file",
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

        protected async Task<TResponse> UploadFileAsyncLocal<TResponse>(string relativePath, string filename, Stream stream, object routeValues = null, string formName = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string uri = null;

            try
            {
                uri = CreateRequestUri(relativePath, routeValues);

                // http://stackoverflow.com/a/10744043/232566

                using (var client = await CreateHttpClientAsync())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        using (var fileContent = new StreamContent(stream))
                        {
                            //fileContent.Headers.Add("Content-Disposition", $"attachment; filename={filename}");

                            if (string.IsNullOrWhiteSpace(formName))
                            {
                                content.Add(fileContent);
                            }
                            else
                            {
                                content.Add(fileContent, formName, filename);
                            }

                            using (var response = await client.PostAsync(uri, content, cancellationToken))
                            {
                                return await ParseResponse<TResponse>(uri, response);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to Post {Uri}", uri);

                throw;
            }
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
