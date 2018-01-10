using System;
using System.Collections.Generic;
using System.Linq;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApi.Common;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/deviceConfiguration")]
    [Authorize]
    public class DeviceConfigurationController : DeviceControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceConfigurationController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        [Produces(typeof(GetDeviceConfigurationResponse))]
        public IActionResult Get()
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                //Get the device
                var device = connection.Get<Services.Contracts.Device>(DeviceId);

                if (device == null)
                    return NotFound("Unable to find device.");

                var application = connection.Get<Application>(device.ApplicationId);

                if (application == null)
                    return NotFound("Unable to find application.");

                //Get the environment variables
                var deviceEnvironmentVariables = connection.GetDeviceEnvironmentVariables(DeviceId);
                var applicationEnvironmentVariables = connection.GetApplicationEnvironmentVariables(application.Id);

                //Start out with the version information at the application level.
                var response = new GetDeviceConfigurationResponse()
                {
                    SupervisorVersionId = application.SupervisorVersionId,
                    ApplicationVersionId = application.ApplicationVersionId,
                    RootFileSystemVersionId = application.RootFileSystemVersionId
                };

                //Override with device level version information (if available)
                if (device.SupervisorVersionId != null)
                    response.SupervisorVersionId = device.SupervisorVersionId.Value;

                if (device.ApplicationVersionId != null)
                    response.ApplicationVersionId = device.ApplicationVersionId.Value;

                if (device.RootFileSystemVersionId != null)
                    response.RootFileSystemVersionId = device.RootFileSystemVersionId.Value;

                //Once again, start out with the application level (this time environment variables)
                Dictionary<string, string> effectiveEnvironmentVariables = new Dictionary<string, string>();

                foreach (var variable in applicationEnvironmentVariables)
                {
                    effectiveEnvironmentVariables[variable.Name] = variable.Value;
                }

                //Now add / override with the device level environment variables.
                foreach (var variable in deviceEnvironmentVariables)
                {
                    effectiveEnvironmentVariables[variable.Name] = variable.Value;
                }

                //Copy the effective variables to the response
                response.EnvironmentVariables = effectiveEnvironmentVariables.Select(v => new EnvironmentVariable()
                {
                    Name = v.Key,
                    Value = v.Value
                }).ToArray();

                //We're good
                return Ok(response);
            }   
        }
    }
}