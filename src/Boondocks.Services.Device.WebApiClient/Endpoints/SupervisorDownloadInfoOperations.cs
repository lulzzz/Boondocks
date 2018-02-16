namespace Boondocks.Services.Device.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.WebApiClient;

    public class SupervisorDownloadInfoOperations
    {
        private readonly ApiClient _client;
        private readonly TokenFactory _tokenFactory;

        internal SupervisorDownloadInfoOperations(ApiClient client, TokenFactory tokenFactory)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
        }

        public Task<ImageDownloadInfo> GetSupervisorVersionDownloadInfo(GetImageDownloadInfoRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<ImageDownloadInfo>(
                cancellationToken,
                HttpMethod.Post,
                ResourceUris.SupervisorDownloadInfo,
                headers: _tokenFactory.CreateRequestHeaders(),
                request: request);

        }
    }
}