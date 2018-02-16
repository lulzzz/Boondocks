namespace Boondocks.Services.Device.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.WebApiClient;

    public class HeartbeatOperations
    {
        private readonly ApiClient _client;
        private readonly TokenFactory _tokenFactory;

        internal HeartbeatOperations(ApiClient client, TokenFactory tokenFactory)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
        }

        /// <summary>
        ///     Heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HeartbeatResponse> HeartbeatAsync(HeartbeatRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<HeartbeatResponse>(cancellationToken, HttpMethod.Post,
                ResourceUris.Heartbeat, headers: _tokenFactory.CreateRequestHeaders(), request: request);
        }
    }
}