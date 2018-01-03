using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApiClient;
using Boondocks.Supervisor.Model;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Boondocks.Supervisor
{
    public class SupervisorHost
    {
        private readonly DeviceConfiguration _deviceConfiguration;
        private readonly DeviceApiClient _deviceApiClient;

        public SupervisorHost(DeviceConfiguration deviceConfiguration)
        {
            _deviceConfiguration = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));

            _deviceApiClient = new DeviceApiClient(
                _deviceConfiguration.DeviceId,
                _deviceConfiguration.DeviceKey,
                _deviceConfiguration.DeviceApiUrl);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await EnsureApplicationRunning(cancellationToken);

            TimeSpan pollTime = TimeSpan.FromSeconds(_deviceConfiguration.PollSeconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await HeartbeatAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                await Task.Delay(pollTime, cancellationToken);
            }
        }

        private async Task EnsureApplicationRunning(CancellationToken cancellationToken)
        {
            try
            {
                var dockerClientConfiguration = new DockerClientConfiguration(new Uri(_deviceConfiguration.DockerEndpoint));

                var dockerClient = dockerClientConfiguration.CreateClient();

                var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true,
                }, cancellationToken);

                Console.WriteLine("Containers:");

                foreach (var container in containers)
                {
                    Console.WriteLine($"\t{string.Join(",", container.Names)} [{container.Status}]");
                }

                Console.WriteLine();

                var images = await dockerClient.Images.ListImagesAsync(new ImagesListParameters()
                {
                    All = true
                }, cancellationToken);

                Console.WriteLine("Images:");

                foreach (var image in images)
                {
                    Console.WriteLine($"\t{image.ID}");
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            var response = await _deviceApiClient.HeartbeatAsync(new HeartbeatRequest()
            {
                RootFileSystemVersion = "rfs 1.0.0"

            }, cancellationToken);

            foreach (var envVar in response.EnvironmentVariables)
            {
                Console.WriteLine($"  {envVar.Name}: {envVar.Value}");
            }
        }
    }
}