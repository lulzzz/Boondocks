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

    public class AgentVersionOperations
    {
        private readonly ApiClient _client;

        internal AgentVersionOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<AgentVersion[]> GetAgentVersions(GetAgentVersionsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            dynamic query = new ExpandoObject();

            if (request.DeviceTypeId != null)
                query.deviceTypeId = request.DeviceTypeId.Value;

            return _client.MakeJsonRequestAsync<AgentVersion[]>(
                cancellationToken, 
                HttpMethod.Get, 
                ResourceUrls.AgentVersions, 
                (object) query);
        }

        public Task<AgentVersion> CreateAgentVersion(CreateAgentVersionRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<AgentVersion>(
                cancellationToken, 
                HttpMethod.Post,
                ResourceUrls.AgentVersions,
                request: request);
        }
    }
}