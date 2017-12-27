using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApiClient;

namespace Boondocks.Supervisor
{
    public class SupervisorHost
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var client = new DeviceApiClient("my-device-id", "my-device-key", "http://localhost:54983/");

            var response = await client.HeartbeatAsync(new HeartbeatRequest()
            {
                RootFileSystemVersion = "rfs 1.0.0"

            }, cancellationToken);


            foreach (var envVar in response.EnvironmentVariables)
            {
                Console.WriteLine($"  {envVar.Name}: {envVar.Value}");
            }

            

            await Task.Delay(-1, cancellationToken);
        }
    }
}