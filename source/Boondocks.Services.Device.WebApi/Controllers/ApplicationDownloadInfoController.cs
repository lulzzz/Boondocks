using System;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    using Boondocks.Base;
    using Common;
    using Contracts;
    using Dapper.Contrib.Extensions;
    using DataAccess;
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;
    using Services.Contracts;

    [Produces("application/json")]
    [Route("v1/applicationDownloadInfo")]
    public class ApplicationDownloadInfoController : DeviceControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ApplicationDownloadInfoController> _logger;
        private readonly RegistryConfig _registryConfig;
        private readonly RepositoryNameFactory _repositoryNameFactory;

        public ApplicationDownloadInfoController(
            IDbConnectionFactory connectionFactory, 
            ILogger<ApplicationDownloadInfoController> logger,
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
                var applicationVersion = connection.Get<ApplicationVersion>(request.Id);

                if (applicationVersion == null)
                    return NotFound();

                var device = connection.Get<Device>(DeviceId);

                if (device == null)
                    return NotFound();

                if (device.ApplicationId != applicationVersion.ApplicationId)
                    return BadRequest();

                //Verify that the device has access to this *specific* version.
                var application = connection.Get<Application>(device.ApplicationId);

                if (application.ApplicationVersionId != request.Id && device.ApplicationVersionId != request.Id)
                    return BadRequest();

                var response = new ImageDownloadInfo()
                {
                    ImageId = applicationVersion.ImageId,
                    Registry = _registryConfig.RegistryHost,
                    AuthToken = null, //TODO: This is where the auth token will go
                    Repository = _repositoryNameFactory.FromApplication(device.ApplicationId),
                    Name = applicationVersion.Name,
                };

                //TODO: Add a device event for getting this token

                return Ok(response);
            }   
        }
    }
}