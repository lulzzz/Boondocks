namespace Boondocks.Services.Management.WebApiClient.Endpoints
{
    using System;
    using Services.WebApiClient;

    public class DeviceEnvironmentVariableOperations
    {
        private readonly ApiClient _client;

        internal DeviceEnvironmentVariableOperations(ApiClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
}