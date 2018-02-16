using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.WebApiClient;

namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    public class ApplicationUploadInfoOperations
    {
        private readonly ApiClient _client;

        internal ApplicationUploadInfoOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<GetUploadInfoResponse> GetApplicationUploadInfo(GetApplicationUploadInfoRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<GetUploadInfoResponse>(cancellationToken, HttpMethod.Post,
                ResourceUrls.ApplicationUploadInfo, request: request);
        }
    }
}