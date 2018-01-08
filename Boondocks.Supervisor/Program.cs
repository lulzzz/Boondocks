using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace Boondocks.Supervisor
{
    class Program
    {
        private static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Starting...");

            //Create the container
            using (var container = ApplicationContainerFactory.Create())
            {
                //Get the supervisor host
                var host = container.Resolve<SupervisorHost>();

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                //We shall cancel on the keypress
                Console.CancelKeyPress += (sender, eventArgs) => cancellationTokenSource.Cancel();

                try
                {
                    await host.RunAsync(cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                }
            }

            return 0;
        }
    }
}
