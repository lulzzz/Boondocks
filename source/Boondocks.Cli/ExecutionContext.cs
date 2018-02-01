namespace Boondocks.Cli
{
    using System;
    using Services.Management.WebApiClient;

    public class ExecutionContext
    {
        private readonly Lazy<ManagementApiClient> _client;

        public ExecutionContext(string endpointUrl)
        {
            _client = new Lazy<ManagementApiClient>(() => new ManagementApiClient(endpointUrl));
        }

        public ManagementApiClient Client => _client.Value;
    }
}