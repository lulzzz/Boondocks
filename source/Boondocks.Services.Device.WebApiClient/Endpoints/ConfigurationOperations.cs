using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.WebApiClient;
using Newtonsoft.Json;

namespace Boondocks.Services.Device.WebApiClient.Endpoints
{
    public class ConfigurationOperations
    {
        private readonly ApiClient _client;
        private readonly TokenFactory _tokenFactory;

        internal ConfigurationOperations(ApiClient client, TokenFactory tokenFactory)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
        }

        /// <summary>
        /// Gets the configuration for the device.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<GetDeviceConfigurationResponse> GetConfigurationAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var response = await _client.MakeRequestAsync(cancellationToken, HttpMethod.Get, ResourceUris.DeviceConfiguration, headers: _tokenFactory.CreateRequestHeaders());

            return JsonConvert.DeserializeObject<GetDeviceConfigurationResponse>(response.Body);
        }
    }
}