namespace Boondocks.Cli.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;

    [Verb("health", HelpText = "Checks the health of the service and its subsystems.")]
    public class HealthCommand : CommandBase
    {
        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var response = await context.Client.Health.GetHealth(cancellationToken);

            int passedCount = response.Items.Count(i => i.Passed);
            int failedCount = response.Items.Count(i => !i.Passed);

            Console.WriteLine($"{passedCount} passed, {failedCount} failed:");

            foreach (var item in response.Items)
            {
                string resultText = item.Passed ? "Passed" : "FAILED";

                Console.WriteLine($"  [{resultText}] {item.Name}: {item.Message}");
            }

            return 0;
        }
    }
}