using System;
using Boondocks.Services.Management.WebApiClient;

namespace Boondocks.Cli
{
    public class ExecutionContext
    {
        private readonly Lazy<ManagementApiClient> _client;

        public ExecutionContext()
        {
            _client = new Lazy<ManagementApiClient>(() => new ManagementApiClient("http://localhost:54985/"));
        }

        public ManagementApiClient Client => _client.Value;

    }
}