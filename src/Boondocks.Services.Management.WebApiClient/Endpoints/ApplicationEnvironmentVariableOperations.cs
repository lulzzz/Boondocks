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

    public class ApplicationEnvironmentVariableOperations
    {
        private readonly ApiClient _client;

        internal ApplicationEnvironmentVariableOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<ApplicationEnvironmentVariable> CreateEnvironmentVariableAsync(CreateApplicationEnvironmentVariableRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<ApplicationEnvironmentVariable>(
                cancellationToken, 
                HttpMethod.Post,
                ResourceUrls.ApplicationEnvironmentVariables,
                request: request);
        }

        public Task<ApplicationEnvironmentVariable[]> GetEnvironmentVariables(Guid applicationId, 
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<ApplicationEnvironmentVariable[]>(
                cancellationToken,
                HttpMethod.Get, 
                ResourceUrls.ApplicationEnvironmentVariables,
                new { applicationId });
        }

        public Task<ApplicationEnvironmentVariable> GetEnvironmentVariable(Guid id,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<ApplicationEnvironmentVariable>(
                cancellationToken, HttpMethod.Get,
                ResourceUrls.ApplicationEnvironmentVariables,
                new {id});
        }

        public Task DeleteEnvironmentVariable(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Delete,
                ResourceUrls.ApplicationEnvironmentVariables,
                new {id});
        }

        public Task UpdateEnvironmentVariable(ApplicationEnvironmentVariable variable,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Put,
                ResourceUrls.ApplicationEnvironmentVariables,
                content: variable.ToJsonContent());
        }
    }

    
}