namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Dynamic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using DataAccess.Domain;
    using Services.WebApiClient;

    public class SupervisorVersionOperations
    {
        private readonly ApiClient _client;

        internal SupervisorVersionOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<SupervisorVersion[]> GetSupervisorVersions(GetSupervisorVersionsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            dynamic query = new ExpandoObject();

            if (request.DeviceArchitectureId != null)
                query.deviceTypeId = request.DeviceArchitectureId.Value;

            return _client.MakeJsonRequestAsync<SupervisorVersion[]>(
                cancellationToken, 
                HttpMethod.Get, 
                ResourceUrls.SupervisorVersions, 
                (object) query);
        }

        public Task<SupervisorVersion> CreateSupervisorVersion(CreateSupervisorVersionRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<SupervisorVersion>(
                cancellationToken, 
                HttpMethod.Post,
                ResourceUrls.SupervisorVersions,
                request: request);
        }
    }
}