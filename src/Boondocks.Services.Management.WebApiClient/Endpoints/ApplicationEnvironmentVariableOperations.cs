namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using Services.WebApiClient;

    public class ApplicationEnvironmentVariableOperations
    {
        private readonly ApiClient _client;

        internal ApplicationEnvironmentVariableOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
}