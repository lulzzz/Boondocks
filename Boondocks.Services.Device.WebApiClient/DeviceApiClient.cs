using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Device.Contracts;

namespace Boondocks.Services.Device.WebApiClient
{
    public class DeviceApiClient : CaptiveAire.WebApiClient.WebApiClient
    {
        private readonly Guid _deviceId;
        private readonly Guid _deviceKey;

        public DeviceApiClient(Guid deviceId, Guid deviceKey, string baseUri)
        {
            _deviceId = deviceId;
            _deviceKey = deviceKey;
            BaseUri = baseUri;
        }

        protected override Task<HttpClient> CreateHttpClientAsync()
        {
            var client = new HttpClient();

            var byteArray = Encoding.UTF8.GetBytes($"{_deviceId:N}:{_deviceKey:N}");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return Task.FromResult(client);
        }

        protected override string BaseUri { get; }

        /// <summary>
        /// Heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HeartbeatResponse> HeartbeatAsync(HeartbeatRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return PostAsync<HeartbeatResponse>("v1/Heartbeat", request, null, cancellationToken);
        }
    }
}