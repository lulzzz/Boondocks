using System;
using System.Linq;
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
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<Device>("select * from Devices order by Name")
                    .ToArray();
            }
        }

        [HttpGet("{filter}")]
        public Device[] Get(string filter)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<Device>("select * from Devices order by Name")
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
                string sql = "insert Devices " +
                             "(" +
                             "  Id, " +
                             "  Name, " +
                             "  ApplicationId, " +
                             "  DeviceKey, " +
                             "  CreatedUtc, " +
                             "  IsDisabled, " +
                             "  ConfigurationVersion" +
                             ") values (" +
                             "  @Id, " +
                             "  @Name, " +
                             "  @ApplicationId, " +
                             "  @DeviceKey, " +
                             "  @CreatedUtc, " +
                             "  0, " +
                             "  @ConfigurationVersion" +
                             ")";

                connection.Execute(sql, device, transaction);

                //TODO: Create the event record               

                transaction.Commit();

                return device;
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromQuery]Guid id, [FromBody]Device device)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                var original = connection.GetDevice(id);

                if (original == null)
                    return NotFound();

                bool applicationChanged = original.ApplicationId != device.ApplicationId;

                if (applicationChanged)
                {
                    //TODO: Check to see if the new application has the same device type.
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
                    "RootFileSystemVersionId = @RootFileSystemVersionId " + 
                    "where Id = @Id";

                //Update the application record
                if (connection.Execute(sql, device, transaction) != 1)
                    return NotFound();

                //TODO: Add device events

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