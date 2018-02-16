namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.WebApiClient;

    public class SupervisorUploadInfoOperations
    {
        private readonly ApiClient _client;

        internal SupervisorUploadInfoOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<GetUploadInfoResponse> GetSupervisorUploadInfo(GetSupervisorUploadInfoRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<GetUploadInfoResponse>(cancellationToken, HttpMethod.Post,
                ResourceUrls.SupervisorUploadInfo, request: request);
        }
    }
}