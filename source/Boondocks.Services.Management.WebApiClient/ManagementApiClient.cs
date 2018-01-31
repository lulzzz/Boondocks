﻿using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Contracts;
using Boondocks.Services.Management.Contracts;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.WebApiClient
{
    public class ManagementApiClient : Services.WebApiClient.WebApiClient
    {
        private static class ResourceUrls
        {
            public const string Applications = "v1/applications";
            public const string ApplicationUploadInfo = "v1/applicationUploadInfo";
            public const string Devices = "v1/devices";
            public const string ApplicationVersions = "v1/applicationVersions";
            public const string DeviceTypes = "v1/deviceTypes";
        }

        public ManagementApiClient(string baseUri)
        {
            BaseUri = baseUri;
        }

        protected override string BaseUri { get; }

        public Task<ApplicationVersion[]> GetApplicationVersionsAsync(GetApplicationVersionsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            dynamic query = new ExpandoObject();

            if (request.ApplicationId != null)
            {
                query.applicationId = request.ApplicationId.Value;
            }

            return GetAsync<ApplicationVersion[]>(ResourceUrls.ApplicationVersions, query, cancellationToken);
        }

        public Task<DeviceType> CreateDeviceTypeAsync(string name, CancellationToken cancellationToken = new CancellationToken())
        {
            var request = new CreateDeviceTypeRequest()
            {
                Name = name
            };

            return PostAsync<DeviceType>(ResourceUrls.DeviceTypes, request, null, cancellationToken);
        }

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

        public async Task<Application> GetApplicationAsync(string name,
            CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO: Add a bloody api call that does this.

            var applications = await GetApplicationsAsync(null, cancellationToken);

            return applications.FirstOrDefault(a => a.Name == name);
        }

        public Task UpdateApplicationAsync(Application application, CancellationToken cancellationToken = new CancellationToken())
        {
            return PutAsync(ResourceUrls.Applications, application, null, cancellationToken);
        }

        public async Task<ApplicationVersion> UploadApplicationVersionAsync(Guid applicationId, string name,
            string imageId, Stream stream, Guid? versionId = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            //Create the uri
            string uri = CreateRequestUri(ResourceUrls.ApplicationVersions, new { });

            var request = new CreateApplicationVersionRequest()
            {
                ApplicationId = applicationId,
                Name = name,
                ImageId = imageId
            };
            
            // http://stackoverflow.com/a/10744043/232566
            using (var client = await CreateHttpClientAsync())
            {
                using (var content = new MultipartFormDataContent())
                {
                    using (var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"))
                    using (var fileContent = new StreamContent(stream))
                    {
                        //Add the content
                        content.Add(stringContent, "request");
                        content.Add(fileContent, "file", $"{request.ImageId.Replace(":", "_")}.image");

                        using (var response = await client.PostAsync(uri, content, cancellationToken))
                        {
                            return await ParseResponse<ApplicationVersion>(uri, response);
                        }
                    }
                }
            }
        }

        public Task<ApplicationUploadInfo> GetApplicationUploadInfo(Guid applicationId, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetAsync<ApplicationUploadInfo>(ResourceUrls.ApplicationUploadInfo , new { id = applicationId}, cancellationToken);
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

        public Task<Device> GetDeviceAsync(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            return GetAsync<Device>(ResourceUrls.Devices, new { id }, cancellationToken);
        }

        public Task UpdateDeviceAsync(Device device, CancellationToken cancellationToken = new CancellationToken())
        {
            return PutAsync(ResourceUrls.Devices, device, null, cancellationToken);
        }

        //protected async Task<TResponse> UploadFileAsyncLocal<TResponse>(string relativePath, string filename, Stream stream, object routeValues = null, string formName = null, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    string uri = null;

        //    try
        //    {
        //        uri = CreateRequestUri(relativePath, routeValues);

        //        // http://stackoverflow.com/a/10744043/232566

        //        using (var client = await CreateHttpClientAsync())
        //        {
        //            using (var content = new MultipartFormDataContent())
        //            {
        //                using (var fileContent = new StreamContent(stream))
        //                {
        //                    //fileContent.Headers.Add("Content-Disposition", $"attachment; filename={filename}");

        //                    if (string.IsNullOrWhiteSpace(formName))
        //                    {
        //                        content.Add(fileContent);
        //                    }
        //                    else
        //                    {
        //                        content.Add(fileContent, formName, filename);
        //                    }

        //                    using (var response = await client.PostAsync(uri, content, cancellationToken))
        //                    {
        //                        return await ParseResponse<TResponse>(uri, response);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex, "Unable to Post {Uri}", uri);

        //        throw;
        //    }
        //}
    }

    public class GetApplicationVersionsRequest
    {
        public Guid? ApplicationId { get; set; }
    }
}
