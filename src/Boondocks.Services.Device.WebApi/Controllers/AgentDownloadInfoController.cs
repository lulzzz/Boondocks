namespace Boondocks.Services.Device.WebApi.Controllers
{
    using System;
    using Base;
    using Common;
    using Contracts;
    using Dapper.Contrib.Extensions;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Services.Contracts;

    [Produces("application/json")]
    [Route("v1/agentDownloadInfo")]
    public class AgentDownloadInfoController : DeviceControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<AgentDownloadInfoController> _logger;
        private readonly RegistryConfig _registryConfig;
        private readonly RepositoryNameFactory _repositoryNameFactory;

        public AgentDownloadInfoController(
            IDbConnectionFactory connectionFactory,
            ILogger<AgentDownloadInfoController> logger,
            RegistryConfig registryConfig,
            RepositoryNameFactory repositoryNameFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _registryConfig = registryConfig ?? throw new ArgumentNullException(nameof(registryConfig));
            _repositoryNameFactory = repositoryNameFactory ?? throw new ArgumentNullException(nameof(repositoryNameFactory));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Gets the download information for a given application version.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces(typeof(ImageDownloadInfo))]
        [Authorize]
        public IActionResult Post([FromBody] GetImageDownloadInfoRequest request)
        {
            //Ensure that the application version exists and that the device has access to it.
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                var agentVersion = connection.Get<AgentVersion>(request.Id);

                if (agentVersion == null)
                    return NotFound(new Error($"Unable to find agent version {request.Id}"));

                var device = connection.Get<Device>(DeviceId);

                if (device == null)
                    return NotFound(new Error("Unable to find device"));

                var application = connection.Get<Application>(device.ApplicationId);

                if (application == null)
                    return NotFound(new Error($"Unable to find application '{device.ApplicationId}'."));

                var deviceType = connection.Get<DeviceType>(application.DeviceTypeId);

                if (deviceType == null)
                    return NotFound(new Error($"Unable to find device type '{application.DeviceTypeId}'."));

                var response = new ImageDownloadInfo()
                {
                    ImageId = agentVersion.ImageId,
                    Registry = _registryConfig.RegistryHost,
                    AuthToken = null, //TODO: This is where the auth token will go
                    Repository = _repositoryNameFactory.Agent,
                    Name = agentVersion.Name,
                };

                //TODO: Add a device event for getting this token

                return Ok(response);
            }
        }
    }
}