namespace Boondocks.Agent.Base
{
    using System;
    using Docker.DotNet.Models;

    internal class Progress : IProgress<JSONMessage>
    {
        public void Report(JSONMessage value)
        {
            Console.WriteLine($"    {value.ProgressMessage}");
        }
    }
}