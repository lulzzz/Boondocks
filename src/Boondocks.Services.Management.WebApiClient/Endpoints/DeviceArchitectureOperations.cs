namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using DataAccess.Domain;
    using Services.WebApiClient;

    public class DeviceArchitectureOperations
    {
        private readonly ApiClient _client;

        internal DeviceArchitectureOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<DeviceArchitecture[]> GetDeviceArchitectures(CancellationToken cancellationToken = new CancellationToken())
        {
            return await _client.MakeJsonRequestAsync<DeviceArchitecture[]>(cancellationToken,
                HttpMethod.Get, ResourceUrls.DeviceArchitectures);
        }

        public Task<DeviceArchitecture> CreateDeviceArchitecture(
            CreateDeviceArchitectureRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceArchitecture>(
                cancellationToken,
                HttpMethod.Post,
                ResourceUrls.DeviceArchitectures,
                request: request);
        }
    }
}