using System;

namespace Boondocks.Agent.RaspberryPi3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Base.Interfaces;
    using Base.Model;

    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Boondocks agent starting...");

            try
            {
                var platformDetector = new PlatformDetector();

                IPathFactory pathFactory;

                if (platformDetector.IsLinux)
                {
                    pathFactory = new LinuxPathFactory();
                }
                else
                {
                    Console.WriteLine("WARNING: Running on Windows.");
                    pathFactory = new WindowsPathFactory();
                }

                Console.WriteLine($"Device config path: {pathFactory.DeviceConfigFile}");
                Console.WriteLine($"Docker endpoint: {pathFactory.DockerEndpoint}");

                var deviceConfigurationProvider = new DeviceConfigurationProvider(pathFactory);

                if (deviceConfigurationProvider.Exists())
                {
                    //Get the device configuration
                    var deviceConfiguration = deviceConfigurationProvider.GetDeviceConfiguration();

                    //Create the container
                    using (var container = ContainerFactory.Create(pathFactory, deviceConfiguration, new RaspberryPiModule()))
                    {
                        //Get the agent host
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
                return 1;
            }
            finally
            {
                Console.WriteLine("Agent exiting.");
            }

            return 0;
        }

        /// <summary>
        /// Print a message to the console and sleep with da fishes.
        /// </summary>
        /// <param name="reason"></param>
        private static void SleepForever(string reason)
        {
            //There is no sense is attempting to run without a configuration.
            Console.Error.WriteLine($"Unable to run: {reason}" );
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
