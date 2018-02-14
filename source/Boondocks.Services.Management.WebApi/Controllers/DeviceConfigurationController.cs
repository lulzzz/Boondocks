using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using System;
    using Contracts;
    using Dapper.Contrib.Extensions;
    using DataAccess;
    using DataAccess.Interfaces;
    using Model;
    using Newtonsoft.Json;
    using Services.Contracts;

    [Produces("application/json")]
    [Route("v1/deviceConfiguration")]
    public class DeviceConfigurationController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ProvisioningConfig _provisioningConfig;

        public DeviceConfigurationController(
            IDbConnectionFactory connectionFactory, 
            RegistryConfig registryConfig,
            ProvisioningConfig provisioningConfig)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _provisioningConfig = provisioningConfig ?? throw new ArgumentNullException(nameof(provisioningConfig));
        }

        [HttpGet("{id}")]
        [Produces(typeof(GetDeviceConfigurationResponse))]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                //Get the device
                var device = connection.Get<Device>(id);

                if (device == null)
                    return NotFound();

                var configuration = new DeviceConfiguration()
                {
                    DeviceKey = device.DeviceKey,
                    DeviceId = device.Id,
                    DeviceApiUrl = _provisioningConfig.DeviceApiUrl,
                    PollSeconds = 60,
                };

                //Craft the response
                var response = new GetDeviceConfigurationResponse()
                {
                    DeviceConfiguration = JsonConvert.SerializeObject(configuration)
                };

                //Put together the configuration
                return Ok(response);
            }
        }
    }
}