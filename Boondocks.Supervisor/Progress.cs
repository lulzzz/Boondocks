using System;
using Docker.DotNet.Models;

namespace Boondocks.Supervisor
{
    public class Progress :  IProgress<JSONMessage>
    {
        public void Report(JSONMessage value)
        {
            Console.WriteLine($"    {value.ProgressMessage}");
        }
    }
}