using System;
using System.Threading.Tasks;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.Management.WebApi.Model;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Get(Guid id)
        {
            //Make sure we can find the application
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                var application = connection.Get<Application>(id);

                if (application == null)
                    return NotFound();
            }

            //Craft the response
            var response = new ApplicationUploadInfo
            { 
                RegistryHost = _registryConfig.RegistryHost,
                Repository = $"{id:D}"
            };

            return Ok(response);
        }
    }
}