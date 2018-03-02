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
    [Route("v1/agentUploadInfo")]
    public class AgentUploadInfoController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly RegistryConfig _registryConfig;
        private readonly RepositoryNameFactory _repositoryNameFactory;

        public AgentUploadInfoController(
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
        public IActionResult Post([FromBody]GetAgentUploadInfoRequest request)
        {
            //Make sure we can find the application
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                var deviceType = connection.Get<DeviceType>(request.DeviceTypeId);

                if (deviceType == null)
                    return NotFound(new Error($"Unable to find device device type '{request.DeviceTypeId}'."));

                if (string.IsNullOrWhiteSpace(request.Name) )
                    return BadRequest(new Error("No name was specified."));

                if (string.IsNullOrWhiteSpace(request.ImageId))
                    return BadRequest(new Error("No image id was specified."));
                
                //Check for duplicate name.
                if (connection.IsAgentVersionNameInUse(request.DeviceTypeId, request.Name))
                {
                    return Ok(new GetUploadInfoResponse
                    {
                        Reason = $"Name '{request.Name}' is already in use for device architecture '{deviceType.Name}'. Specify a new name."
                    });
                }

                //Craft the response
                var response = new GetUploadInfoResponse
                {
                    CanUpload = true,
                    RegistryHost = _registryConfig.RegistryHost,
                    Repository = _repositoryNameFactory.Agent
                };

                return Ok(response);
            }
        }
    }
}