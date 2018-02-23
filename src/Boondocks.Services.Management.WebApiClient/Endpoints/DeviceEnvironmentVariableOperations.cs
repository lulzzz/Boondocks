namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using DataAccess.Domain;
    using Services.WebApiClient;

    public class DeviceEnvironmentVariableOperations
    {
        private readonly ApiClient _client;

        internal DeviceEnvironmentVariableOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<DeviceEnvironmentVariable> CreateEnvironmentVariableAsync(CreateDeviceEnvironmentVariableRequest request,
           CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceEnvironmentVariable>(
                cancellationToken,
                HttpMethod.Post,
                ResourceUrls.DeviceEnvironmentVariables,
                request: request);
        }

        public Task<DeviceEnvironmentVariable[]> GetEnvironmentVariables(Guid deviceId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceEnvironmentVariable[]>(
                cancellationToken,
                HttpMethod.Get,
                ResourceUrls.DeviceEnvironmentVariables,
                new { deviceId });
        }

        public Task<DeviceEnvironmentVariable> GetEnvironmentVariable(Guid id,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeJsonRequestAsync<DeviceEnvironmentVariable>(
                cancellationToken, HttpMethod.Get,
                ResourceUrls.DeviceEnvironmentVariables,
                new { id });
        }

        public Task DeleteEnvironmentVariable(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Delete,
                ResourceUrls.DeviceEnvironmentVariables,
                new { id });
        }

        public Task UpdateEnvironmentVariable(DeviceEnvironmentVariable variable,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _client.MakeRequestAsync(
                cancellationToken,
                HttpMethod.Put,
                ResourceUrls.DeviceEnvironmentVariables,
                content: variable.ToJsonContent());
        }
    }
}