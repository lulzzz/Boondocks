using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Boondocks.Agent.Interfaces;

namespace Boondocks.Agent
{
    class Program
    {
        private static async Task<int> Main(string[] args)
        {
            Console.WriteLine("Starting supervisor...");

            try
            {
                //Create the container
                using (var container = ContainerFactory.Create())
                {
                    //Get the supervisor host
                    var host = container.Resolve<IAgentHost>();

                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                    //We shall cancel on the keypress
                    Console.CancelKeyPress += (sender, eventArgs) => cancellationTokenSource.Cancel();

                    try
                    {
                        //Run the host
                        await host.RunAsync(cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
                Console.WriteLine(ex.ToString());
                return 1;
            }
            finally
            {
                Console.WriteLine("Supervisor exiting.");
            }

            return 0;
        }
    }
}
