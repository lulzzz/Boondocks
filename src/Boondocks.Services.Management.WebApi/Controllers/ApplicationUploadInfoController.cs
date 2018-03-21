using System;
using System.Threading.Tasks;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.Management.WebApi.Model;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.IISUrlRewrite;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using Base;
    using DataAccess.Domain;

    [Produces("application/json")]
    [Route("v1/applicationUploadInfo")]
    public class ApplicationUploadInfoController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly RegistryConfig _registryConfig;
        private readonly RepositoryNameFactory _repositoryNameFactory;

        public ApplicationUploadInfoController(IDbConnectionFactory connectionFactory, RegistryConfig registryConfig, RepositoryNameFactory repositoryNameFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _registryConfig = registryConfig ?? throw new ArgumentNullException(nameof(registryConfig));
            _repositoryNameFactory = repositoryNameFactory;
        }

        [HttpPost]
        [Produces(typeof(GetUploadInfoResponse))]
        public IActionResult Post([FromBody]GetApplicationUploadInfoRequest request)
        {
            //Make sure we can find the application
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                var application = connection.Get<Application>(request.ApplicationId);

                if (application == null)
                    return NotFound(new Error($"Unable to find application '{request.ApplicationId}'."));

                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new Error("No name was specified."));

                if (string.IsNullOrWhiteSpace(request.ImageId))
                    return BadRequest(new Error("No image id was specified."));

                //Check for duplicate name.
                if (connection.IsApplicationVersionNameInUse(request.ApplicationId, request.Name))
                {
                    return Ok(new GetUploadInfoResponse
                    {
                        Reason = $"Name '{request.Name}' is already in use for application '{application.Name}'. Specify a new name."
                    });
                }

                //Craft the response
                var response = new GetUploadInfoResponse
                {
                    CanUpload = true,
                    RegistryHost = _registryConfig.RegistryHost,
                    Repository = _repositoryNameFactory.FromApplication(request.ApplicationId)
                };

                return Ok(response);
            }
        }
    }
}