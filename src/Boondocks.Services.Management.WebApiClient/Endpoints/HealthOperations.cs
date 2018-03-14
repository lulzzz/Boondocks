namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.Contracts;
    using Services.WebApiClient;

    public class HealthOperations
    {
        private readonly ApiClient _client;

        internal HealthOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<GetHealthResponse> GetHealth(CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<GetHealthResponse>(cancellationToken, 
                HttpMethod.Get,
                ResourceUrls.Health);
        }
    }
}