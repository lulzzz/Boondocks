namespace Boondocks.Services.Device.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.WebApiClient;

    public class AgentDownloadInfoOperations
    {
        private readonly ApiClient _client;
        private readonly TokenFactory _tokenFactory;

        internal AgentDownloadInfoOperations(ApiClient client, TokenFactory tokenFactory)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
        }

        public Task<ImageDownloadInfo> GetAgentVersionDownloadInfo(GetImageDownloadInfoRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<ImageDownloadInfo>(
                cancellationToken,
                HttpMethod.Post,
                ResourceUris.AgentDownloadInfo,
                headers: _tokenFactory.CreateRequestHeaders(),
                request: request);

        }
    }
}