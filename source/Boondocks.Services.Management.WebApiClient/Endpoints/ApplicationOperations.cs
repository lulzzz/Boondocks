namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Services.Contracts;
    using Services.WebApiClient;

    public class ApplicationOperations
    {
        private readonly ApiClient _client;

        internal ApplicationOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<Application> CreateApplicationAsync(CreateApplicationRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<Application>(cancellationToken, HttpMethod.Post,
                ResourceUrls.Applications, request: request);
        }

        public Task<Application[]> GetApplicationsAsync(GetApplicationsRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            dynamic query = new ExpandoObject();

            if (request.DeviceTypeId != null)
                query.deviceTypeId = request.DeviceTypeId.Value;

            return _client.MakeJsonRequestAsync<Application[]>(cancellationToken, HttpMethod.Get,
                ResourceUrls.Applications, (object) query, request: request);
        }

        public Task<Application> GetApplicationAsync(Guid id,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<Application>(cancellationToken, HttpMethod.Get,
                ResourceUrls.Applications, new {id});
        }

        public async Task<Application> GetApplicationAsync(string name,
            CancellationToken cancellationToken = new CancellationToken())
        {
            //TODO: Add a bloody api call that does this.

            var applications = await GetApplicationsAsync(new GetApplicationsRequest(), cancellationToken);

            return applications.FirstOrDefault(a => a.Name == name);
        }

        public Task UpdateApplicationAsync(Application application,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(cancellationToken, HttpMethod.Put, ResourceUrls.Applications,
                content: application.ToJsonContent());
        }
    }
}