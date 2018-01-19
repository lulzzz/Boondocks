using System;
using Boondocks.Services.Management.WebApiClient;

namespace Boondocks.Cli
{
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