using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using System;
    using Base;
    using Contracts;
    using Dapper.Contrib.Extensions;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;
    using Services.Contracts;

    [Produces("application/json")]
    [Route("v1/supervisorUploadInfo")]
    public class SupervisorUploadInfoController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly RegistryConfig _registryConfig;
        private readonly RepositoryNameFactory _repositoryNameFactory;

        public SupervisorUploadInfoController(
            IDbConnectionFactory connectionFactory, 
            RegistryConfig registryConfig,
            RepositoryNameFactory repositoryNameFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _registryConfig = registryConfig ?? throw new ArgumentNullException(nameof(registryConfig));
            _repositoryNameFactory = repositoryNameFactory;
        }

        [HttpPost]
        [Produces(typeof(GetUploadInfoResponse))]
        public IActionResult Post([FromBody]GetSupervisorUploadInfoRequest request)
        {
            //Make sure we can find the application
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                var deviceArchitecture = connection.Get<DeviceArchitecture>(request.DeviceArchitectureId);

                if (deviceArchitecture == null)
                    return NotFound(new Error($"Unable to find device architecture '{request.DeviceArchitectureId}'."));

                if (string.IsNullOrWhiteSpace(request.Name) )
                    return BadRequest(new Error("No name was specified."));

                if (string.IsNullOrWhiteSpace(request.ImageId))
                    return BadRequest(new Error("No image id was specified."));
                
                //Check for duplicate name.
                if (connection.IsSupervisorVersionNameInUse(request.DeviceArchitectureId, request.Name))
                {
                    return Ok(new GetUploadInfoResponse
                    {
                        Reason = $"Name '{request.Name}' is already in use for device architecture '{deviceArchitecture.Name}'. Specify a new name."
                    });
                }

                //Craft the response
                var response = new GetUploadInfoResponse
                {
                    CanUpload = true,
                    RegistryHost = _registryConfig.RegistryHost,
                    Repository = _repositoryNameFactory.Supervisor
                };

                return Ok(response);
            }
        }
    }
}