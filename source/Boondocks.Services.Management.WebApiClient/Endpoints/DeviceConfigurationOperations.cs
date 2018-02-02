namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.WebApiClient;

    public class DeviceConfigurationOperations
    {
        private readonly ApiClient _client;

        internal DeviceConfigurationOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> GetDeviceConfigurationAsync(Guid deviceId, CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _client.MakeJsonRequestAsync<GetDeviceConfigurationResponse>(cancellationToken, HttpMethod.Get, ResourceUrls.DeviceConfiguration,
                new { id = deviceId });

            return response.DeviceConfiguration;
        }
    }
}