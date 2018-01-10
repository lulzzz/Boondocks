using System.Threading.Tasks;
using CommandLine;

namespace BoondocksCli.Commands
{
    [Verb("rename-app", HelpText = "Rename an application")]
    public class RenameApplicationOptions : OptionsBase
    {
        [Option('a', "application", Required = true, HelpText = "The application uuid.")]
        public string ApplicationId { get; set; }

        [Option('n', "name", Required = true, HelpText = "The new name of the application.")]
        public string Name { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            //Console.WriteLine($"Deploy app: {ApplicationId} {ImageId}");

            

            return 0;
        }
    }
}