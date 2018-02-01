namespace Boondocks.Cli
{
    using System.Linq;
    using CommandLine;

    internal class Program
    {
        private static int Main(string[] args)
        {
            //All commands are based off of this type.
            var baseType = typeof(CommandBase);

            //Get the command types via reflection.
            var commandTypes = baseType.Assembly.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

            //Do it now
            return Parser.Default.ParseArguments(args, commandTypes)
                .MapResult(
                    (CommandBase opts) => opts.ExecuteAsync().GetAwaiter().GetResult(),
                    errs => 1);
        }
    }
}