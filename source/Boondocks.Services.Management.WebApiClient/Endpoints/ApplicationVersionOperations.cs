using System;
using System.Dynamic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Contracts;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.WebApiClient;

namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    public class ApplicationVersionOperations
    {
        private readonly NetworkClient _client;

        internal ApplicationVersionOperations(NetworkClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<ApplicationVersion[]> GetApplicationVersionsAsync(GetApplicationVersionsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            dynamic query = new ExpandoObject();

            if (request.ApplicationId != null)
            {
                query.applicationId = request.ApplicationId.Value;
            }

            return _client.MakeJsonRequestAsync<ApplicationVersion[]>(cancellationToken, HttpMethod.Get, ResourceUrls.ApplicationVersions, (object)query);           
        }

        public Task<ApplicationVersion> UploadApplicationVersionAsync(CreateApplicationVersionRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {

            return _client.MakeJsonRequestAsync<ApplicationVersion>(cancellationToken, HttpMethod.Post,
                ResourceUrls.ApplicationVersions, request: request);
        }

       
    }
}