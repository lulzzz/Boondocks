using System;
using System.Linq;
using Boondocks.Services.Base;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.Management.WebApi.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/device")]
    public class DevicesController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DevicesController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public Device[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<Device>("select * from Devices", Request.Query);

            queryBuilder.AddGuidParameter("applicationId", "ApplicationId");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder.Execute(connection)
                    .ToArray();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .GetDevice(id)
                    .ObjectOrNotFound();
            }
        }

        [HttpPost]
        public Device Post(CreateDeviceRequest request)
        {
            var device = new Device()
            {
                Name = request.Name,
                ApplicationId = request.ApplicationId,
                DeviceKey = request.DeviceKey ?? Guid.NewGuid(),
                ConfigurationVersion = Guid.NewGuid()
            }.SetNew();

            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                connection.InsertDevice(transaction, request.Name, request.ApplicationId, request.DeviceKey);

                //Create the event record               
                connection.InsertDeviceEvent(transaction, device.Id, DeviceEventType.Created, "Device created.");

                transaction.Commit();

                return device;
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody]Device device)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                var original = connection.GetDevice(device.Id);

                if (original == null)
                    return NotFound();

                //Check to see if we're trying to move the device to another application.
                bool applicationChanged = original.ApplicationId != device.ApplicationId;

                //We don't need to get these in call cases
                Lazy<Application> oldApplication = new Lazy<Application>(() => connection.GetApplication(original.ApplicationId));
                Lazy<Application> newApplication = new Lazy<Application>(() => connection.GetApplication(device.ApplicationId));

                if (applicationChanged)
                {
                    //TODO: Check to see if the new application has the same device type.
                    if (oldApplication.Value.DeviceTypeId != newApplication.Value.DeviceTypeId)
                    {
                        //TODO: Log this
                        return StatusCode(500);
                    }
                }

                bool configuredChanged = applicationChanged
                    || original.ApplicationVersionId != device.ApplicationVersionId
                    || original.SupervisorVersionId != device.SupervisorVersionId
                    || original.RootFileSystemVersionId != device.RootFileSystemVersionId;

                if (configuredChanged)
                {
                    device.ConfigurationVersion = Guid.NewGuid();
                }
                else
                {
                    device.ConfigurationVersion = original.ConfigurationVersion;
                }

                string sql =
                    "update Devices set Name = @Name, " + 
                    "ApplicationId = @ApplicationId, " + 
                    "ApplicationVersionId = @ApplicationVersionId, " + 
                    "SupervisorVersionId = @SupervisorVersionId, " +  
                    "RootFileSystemVersionId = @RootFileSystemVersionId," + 
                    "ConfigurationVersion = @ConfigurationVersion " +
                    "where Id = @Id";

                //Update the application record
                if (connection.Execute(sql, device, transaction) != 1)
                    return NotFound();

                //Add device events
                if (original.Name != device.Name)
                {
                    connection.InsertDeviceEvent(
                        transaction, 
                        device.Id, 
                        DeviceEventType.Renamed, 
                        $"Device renamed from '{original.Name}' to '{device.Name}'.");
                }

                if (applicationChanged)
                {
                    connection.InsertDeviceEvent(
                        transaction,
                        device.Id,
                        DeviceEventType.Moved,
                        $"Device moved from '{oldApplication.Value.Name}' to '{newApplication.Value.Name}'.");
                }

                transaction.Commit();
            }

            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                connection.Execute("delete DeviceTypes where Id = @id", new { id })
                    .HandleUpdateResult();
            }

            return Ok();
        }
    }
}