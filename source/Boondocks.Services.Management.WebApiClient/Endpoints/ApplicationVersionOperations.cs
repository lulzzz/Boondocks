namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Dynamic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.Contracts;
    using Services.WebApiClient;

    public class ApplicationVersionOperations
    {
        private readonly ApiClient _client;

        internal ApplicationVersionOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<ApplicationVersion[]> GetApplicationVersionsAsync(GetApplicationVersionsRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            dynamic query = new ExpandoObject();

            if (request.ApplicationId != null) query.applicationId = request.ApplicationId.Value;

            return _client.MakeJsonRequestAsync<ApplicationVersion[]>(cancellationToken, HttpMethod.Get,
                ResourceUrls.ApplicationVersions, (object) query);
        }

        public Task<ApplicationVersion> UploadApplicationVersionAsync(CreateApplicationVersionRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<ApplicationVersion>(cancellationToken, HttpMethod.Post,
                ResourceUrls.ApplicationVersions, request: request);
        }
    }
}