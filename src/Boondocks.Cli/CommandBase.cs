namespace Boondocks.Cli
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;

    public abstract class CommandBase
    {
        private const string DefaultEndpointUrl = "http://localhost/Boondocks.Services.Management.WebApi/";

        [Option('e', "endpoint-url", HelpText = "Specify the management API endpoint. Can also be set in the BOONDOCKS_URL environment variable.")]
        public string EndpointUrl { get; set; }

        protected abstract Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Allows the url to come from the environment
        /// </summary>
        /// <returns></returns>
        private string GetEffectiveEndpointUrl()
        {
            //Is it specified on the commmand line (always takes precedence)
            if (!string.IsNullOrWhiteSpace(EndpointUrl))
                return EndpointUrl;

            //Is it specified as an environment variable.
            string value = Environment.GetEnvironmentVariable("BOONDOCKS_URL");

            if (!string.IsNullOrWhiteSpace(value))
                return value;
            
            return DefaultEndpointUrl;
        }

        public Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            string url = GetEffectiveEndpointUrl();

            var context = new CommandContext(url);

            return ExecuteAsync(context, cancellationToken);
        }
    }
}