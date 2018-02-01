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
    [Produces("application/json")]
    [Route("v1/applicationUploadInfo")]
    public class ApplicationUploadInfoController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly RegistryConfig _registryConfig;

        public ApplicationUploadInfoController(IDbConnectionFactory connectionFactory, RegistryConfig registryConfig)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _registryConfig = registryConfig ?? throw new ArgumentNullException(nameof(registryConfig));
        }

        [HttpGet("{id}")]
        [Produces(typeof(GetApplicationUploadInfoResponse))]
        public IActionResult Get(GetApplicationUploadInfoRequest request)
        {
            //Make sure we can find the application
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                var application = connection.Get<Application>(request.ApplicationId);

                if (application == null)
                    return NotFound();

                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ImageId))
                {
                    return BadRequest();
                }

                if (connection.IsApplicationVersionNameInUse(request.ApplicationId, request.Name))
                {
                    return Ok(new GetApplicationUploadInfoResponse
                    {
                        Reason = $"Name '{request.Name}' is already in use."
                    });
                }

                if (connection.IsApplicationVersionNameInUse(request.ApplicationId, request.ImageId))
                {
                    return Ok(new GetApplicationUploadInfoResponse
                    {
                        Reason = $"Image '{request.ImageId}' has already been uploaded."
                    });
                }

                //Craft the response
                var response = new GetApplicationUploadInfoResponse
                {
                    CanUpload = true,
                    RegistryHost = _registryConfig.RegistryHost,
                    Repository = RepositoryNameFactory.Create(request.ApplicationId)
                };

                return Ok(response);
            }
        }
    }
}