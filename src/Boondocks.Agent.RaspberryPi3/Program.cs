using System;

namespace Boondocks.Agent.RaspberryPi3
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Base;
    using Base.Interfaces;
    using Base.Model;
    using Serilog;

    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Boondocks agent starting...");

            try
            {
                var platformDetector = new PlatformDetector();
                var pathFactory = new PathFactory(platformDetector);
                var deviceConfigurationProvider = new DeviceConfigurationProvider(pathFactory);

                if (deviceConfigurationProvider.Exists())
                {
                    //Get the device configuration
                    var deviceConfiguration = deviceConfigurationProvider.GetDeviceConfiguration();

                    //Create the container
                    using (var container = ContainerFactory.Create(pathFactory, deviceConfiguration))
                    {
                        LogRootFileSystemInfo(container);

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
                    Console.Error.WriteLine("Unable to find device configuration. Unable to run.");

                    Thread.Sleep(Timeout.Infinite);
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

        private static void LogRootFileSystemInfo(ILifetimeScope lifetimeScope)
        {
            try
            {
                var logger = lifetimeScope.Resolve<ILogger>();

                var rootFileSysteUpdater = new RootFileSystemUpdater(logger);

                int? partition = rootFileSysteUpdater.GetCurrentPartition();

                Console.WriteLine($" Patition: {partition}");

                string rootFileSystemVersion = rootFileSysteUpdater.GetImageVersionInfo();

                Console.WriteLine($" Root file syste version: {rootFileSystemVersion}");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
