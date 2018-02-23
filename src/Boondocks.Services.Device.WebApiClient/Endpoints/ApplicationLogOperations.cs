namespace Boondocks.Services.Device.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.WebApiClient;

    public class ApplicationLogOperations
    {
        private readonly ApiClient _client;
        private readonly TokenFactory _tokenFactory;

        internal ApplicationLogOperations(ApiClient client, TokenFactory tokenFactory)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
        }

        public Task UploadLogsAsync(SubmitApplicationLogsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Post,
                ResourceUris.ApplicationLogs,
                headers: _tokenFactory.CreateRequestHeaders(),
                content: request.ToJsonContent());
        }
    }
}