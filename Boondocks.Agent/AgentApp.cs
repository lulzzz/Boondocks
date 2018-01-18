using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using BizArk.ConsoleApp;
using Boondocks.Agent.Domain;
using Boondocks.Agent.Interfaces;
using Boondocks.Base;

namespace Boondocks.Agent
{
    internal class AgentApp : BaseConsoleApp
    {
        [Description("A custom device api url.")]
        [CmdLineArg(ShowInUsage = true)]
        public string DeviceApiUrl { get; set; }

        [Description("A custom device id.")]
        [CmdLineArg(ShowInUsage = true)]
        public string DeviceId { get; set; }

        [Description("A custom device key.")]
        [CmdLineArg(ShowInUsage = true)]
        public string DeviceKey { get; set; }

        [Description("A custom docker endpoint.")]
        [CmdLineArg(ShowInUsage = true)]
        public string DockerEndpoint { get; set; }

        [Description("Custom poll seconds")]
        [CmdLineArg(ShowInUsage = true)]
        public string PollSeconds { get; set; }

        public override int Start()
        {
            Console.WriteLine("Starting supervisor...");

            try
            {
                //Get the override settings from the command line
                var deviceConfigurationOverride = new DeviceConfigurationOverride
                {
                    DeviceApiUrl = DeviceApiUrl,
                    DeviceId = DeviceId.TryParseGuid(),
                    DeviceKey = DeviceKey.TryParseGuid(),
                    DockerEndpoint = DockerEndpoint,
                    PollSeconds = PollSeconds.TryParseInt()
                };

                //Create the container
                using (var container = ContainerFactory.Create(deviceConfigurationOverride))
                {
                    //Get the supervisor host
                    var host = container.Resolve<IAgentHost>();

                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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