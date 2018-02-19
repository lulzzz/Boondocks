namespace Boondocks.Agent
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Interfaces;

    internal class Program
    {
        private const int StartupSeconds = 10;

        private static int Main(string[] args)
        {
            Console.WriteLine("Agent starting...");

            try
            {
                //Create the container
                using (var container = ContainerFactory.Create())
                {
                    //Get the supervisor host
                    var host = container.Resolve<IAgentHost>();

                    var cancellationTokenSource = new CancellationTokenSource();

                    //We shall cancel on the keypress
                    Console.CancelKeyPress += (sender, eventArgs) => cancellationTokenSource.Cancel();

                    try
                    {
                        //Run the host
                        host.RunAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
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