namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;

    [Verb("import-root-file-system-ver", HelpText = "Imports a version of the root file system for a given device type.")]
    public class ImportRootFileSystemVersionCommand : CommandBase
    {
        protected override Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}