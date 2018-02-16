namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Dynamic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using DataAccess.Domain;
    using Services.Contracts;
    using Services.WebApiClient;

    public class DeviceOperations
    {
        private readonly ApiClient _client;

        internal DeviceOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<Device> CreateDeviceAsync(CreateDeviceRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<Device>(cancellationToken, HttpMethod.Post,
                ResourceUrls.Devices, request: request);
        }

        public Task<Device[]> GetDevicesAsync(GetDevicesRequest request = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (request == null)
                request = new GetDevicesRequest();

            dynamic query = new ExpandoObject();

            if (request.ApplicationId != null) query.applicationId = request.ApplicationId.Value;

            return _client.MakeJsonRequestAsync<Device[]>(cancellationToken, HttpMethod.Get, ResourceUrls.Devices,
                (object) query, request: request);
        }

        public Task<Device> GetDeviceAsync(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<Device>(cancellationToken, HttpMethod.Get, ResourceUrls.Devices,
                new {id});
        }

        public Task UpdateDeviceAsync(Device device, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(cancellationToken, HttpMethod.Put, ResourceUrls.Devices,
                content: device.ToJsonContent());
        }
    }
}