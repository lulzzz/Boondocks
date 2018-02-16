namespace Boondocks.Services.Device.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Contracts;
    using Dapper.Contrib.Extensions;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;

    [Authorize]
    [Produces("application/json")]
    [Route("v1/deviceConfiguration")]
    public class DeviceConfigurationController : DeviceControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceConfigurationController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Gets the effective device configuration considering both application and device level settings.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(typeof(GetDeviceConfigurationResponse))]
        public IActionResult Get()
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                //TODO: Consider executing a single command with multiple result sets.
                //      Might have to use some T-SQL to get the application given the device id.

                //Get the device
                var device = connection.Get<Device>(DeviceId);

                if (device == null)
                    return NotFound("Unable to find device.");

                var application = connection.Get<Application>(device.ApplicationId);

                if (application == null)
                    return NotFound("Unable to find application.");

                //Get the environment variables
                var deviceEnvironmentVariables = connection.GetDeviceEnvironmentVariables(DeviceId);
                var applicationEnvironmentVariables = connection.GetApplicationEnvironmentVariables(application.Id);

                var applicationVersionId = application.ApplicationVersionId;
                var supervisorVersionId = application.SupervisorVersionId;

                //Start out with the version information at the application level.
                var response = new GetDeviceConfigurationResponse
                {
                    RootFileSystemVersionId = application.RootFileSystemVersionId,
                    ConfigurationVersion = device.ConfigurationVersion
                };

                //Override with device level version information (if available)
                if (device.SupervisorVersionId != null)
                    supervisorVersionId = device.SupervisorVersionId.Value;

                if (device.ApplicationVersionId != null)
                    applicationVersionId = device.ApplicationVersionId.Value;

                if (device.RootFileSystemVersionId != null)
                    response.RootFileSystemVersionId = device.RootFileSystemVersionId.Value;

                //Get the detailed application version information
                if (applicationVersionId != null)
                {
                    var applicationVersion = connection.Get<ApplicationVersion>(applicationVersionId.Value);

                    if (applicationVersion == null)
                        return StatusCode(StatusCodes.Status500InternalServerError);

                    //Set thhe image reference
                    response.ApplicationVersion = new VersionReference
                    {
                        Id = applicationVersion.Id,
                        ImageId = applicationVersion.ImageId,
                        Name = applicationVersion.Name
                    };
                }

                //Get the detailed supervisor version id
                if (supervisorVersionId != null)
                {
                    var supervisorVersion = connection.Get<SupervisorVersion>(supervisorVersionId);

                    if (supervisorVersion == null)
                        return StatusCode(StatusCodes.Status500InternalServerError);

                    response.SupervisorVersion = new VersionReference
                    {
                        Id = supervisorVersion.Id,
                        ImageId = supervisorVersion.ImageId,
                        Name = supervisorVersion.Name
                    };
                }

                //Once again, start out with the application level (this time environment variables)
                var effectiveEnvironmentVariables = new Dictionary<string, string>();

                foreach (var variable in applicationEnvironmentVariables)
                    effectiveEnvironmentVariables[variable.Name] = variable.Value;

                //Now add / override with the device level environment variables.
                foreach (var variable in deviceEnvironmentVariables)
                    effectiveEnvironmentVariables[variable.Name] = variable.Value;

                //Copy the effective variables to the response
                response.EnvironmentVariables = effectiveEnvironmentVariables.Select(v => new EnvironmentVariable
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