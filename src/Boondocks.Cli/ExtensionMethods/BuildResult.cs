namespace Boondocks.Cli.ExtensionMethods
{
    using System.Collections.Generic;
    using System.Text;

    internal class BuildResult
    {
        public IList<string> Ids { get; } = new List<string>();

        public IList<string> Messages { get; } = new List<string>();

        public IList<string> Errors { get; } = new List<string>();

        public override string ToString()
        {
            var output = new StringBuilder();

            foreach (var message in Messages) output.Append(message);

            return output.ToString();
        }
    }
}