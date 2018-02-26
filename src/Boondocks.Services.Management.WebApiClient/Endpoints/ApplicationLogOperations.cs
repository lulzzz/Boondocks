namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using DataAccess.Domain;
    using Services.WebApiClient;

    public class ApplicationLogOperations
    {
        private readonly ApiClient _client;

        internal ApplicationLogOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<ApplicationLog[]> GetApplicationLogsAsync(Guid deviceId, CancellationToken cancellationToken)
        {
            return _client.MakeJsonRequestAsync<ApplicationLog[]>(cancellationToken, HttpMethod.Get, ResourceUrls.ApplicationLogs, new
            {
                deviceId
            });
        }
    }
}