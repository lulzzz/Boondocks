namespace Boondocks.Cli
{
    using System;
    using System.Linq;
    using System.Threading;
    using CommandLine;

    internal class Program
    {
        private static int Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, a) => cts.Cancel();

            //All commands are based off of this type.
            var baseType = typeof(CommandBase);

            //Get the command types via reflection.
            var commandTypes = baseType.Assembly.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

            //Do it now
            return Parser.Default.ParseArguments(args, commandTypes)
                .MapResult(
                    (CommandBase opts) => opts.ExecuteAsync(cts.Token).GetAwaiter().GetResult(),
                    errs => 1);
        }
    }
}