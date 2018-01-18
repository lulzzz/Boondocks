using System;
using Docker.DotNet.Models;

namespace Boondocks.Agent
{
    public class Progress :  IProgress<JSONMessage>
    {
        public void Report(JSONMessage value)
        {
            Console.WriteLine($"    {value.ProgressMessage}");
        }
    }
}