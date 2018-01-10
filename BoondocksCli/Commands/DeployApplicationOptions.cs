using System;
using System.Threading.Tasks;
using Boondocks.Services.Management.Contracts;
using CommandLine;

namespace BoondocksCli.Commands
{
    [Verb("deploy-app", HelpText = "Deploys an application to the server")]
    public class DeployApplicationOptions : OptionsBase
    {
        [Option('a', "application", Required = true, HelpText = "The application uuid.")]
        public string ApplicationId { get; set; }

        [Option('i', "image", Required = true, HelpText = "The image uuid.")]
        public string ImageUuid { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}