using System;

namespace Boondocks.Bootstrap
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agent.Base;
    using Agent.Base.Interfaces;
    using Agent.Base.Model;
    using Autofac;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Boondocks bootstrap starting...");

            try
            {
                var platformDetector = new PlatformDetector();

                IPathFactory pathFactory = platformDetector.CreatePathFactory();

                var deviceConfigurationProvider = new DeviceConfigurationProvider(pathFactory);

                if (deviceConfigurationProvider.Exists())
                {
                    //Get the device configuration
                    var deviceConfiguration = deviceConfigurationProvider.GetDeviceConfiguration();

                    //Create the container
                    using (var container = ContainerFactory.Create(pathFactory, deviceConfiguration, new BootstrapModule()))
                    {
                        //Get the agent host
                        var host = container.Resolve<BootstrapHost>();

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
                else
                {
                    //There is no sense is attempting to run without a configuration.
                    SleepForever("Unable to find device configuration.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Bootstrap exiting.");
            }
        }

        /// <summary>
        /// Print a message to the console and sleep with da fishes.
        /// </summary>
        /// <param name="reason"></param>
        private static void SleepForever(string reason)
        {
            //There is no sense is attempting to run without a configuration.
            Console.Error.WriteLine($"Bootstrap unable to run: {reason}" );
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
