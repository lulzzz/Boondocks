namespace Boondocks.Cli
{
    using System;
    using Services.Management.WebApiClient;

    public class CommandContext
    {
        private readonly Lazy<ManagementApiClient> _client;

        public CommandContext(string endpointUrl)
        {
            _client = new Lazy<ManagementApiClient>(() => new ManagementApiClient(endpointUrl));
        }

        public ManagementApiClient Client => _client.Value;
    }
}