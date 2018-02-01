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
        private readonly NetworkClient _client;

        internal ApplicationUploadInfoOperations(NetworkClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<ApplicationUploadInfo> GetApplicationUploadInfo(Guid applicationId, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<ApplicationUploadInfo>(cancellationToken, HttpMethod.Get,
                ResourceUrls.ApplicationUploadInfo, new {id = applicationId});
        }
    }
}