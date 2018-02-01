namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.Contracts;
    using Services.WebApiClient;

    public class DeviceTypeOperations
    {
        private readonly ApiClient _client;

        internal DeviceTypeOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<DeviceType> CreateDeviceTypeAsync(CreateDeviceTypeRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceType>(cancellationToken, HttpMethod.Post,
                ResourceUrls.DeviceTypes, request: request);
        }

        public Task<DeviceType[]> GetDeviceTypesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceType[]>(cancellationToken, HttpMethod.Get,
                ResourceUrls.DeviceTypes);
        }
    }
}