using System;
using System.Data;
using System.Linq;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.Management.WebApi.Model;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using DataAccess.Domain;

    [Produces("application/json")]
    [Route("v1/devices")]
    public class DevicesController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DevicesController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Can add query parameters such as applicationId, isDisabled and isDeleted.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Device[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<Device>("select * from Devices", Request.Query);

            queryBuilder.TryAddGuidParameter("applicationId", "ApplicationId");
            queryBuilder.TryAddBitParameter("isDisabled", "IsDisabled");

            //Ignore deleted devices, unless the caller specifically asks for them.
            if (!queryBuilder.TryAddBitParameter("isDeleted", "IsDeleted"))
            {
                queryBuilder.AddCondition("IsDeleted", false);
            }

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder.Execute(connection)
                    .ToArray();
            }
        }

        [HttpGet("{id}")]
        [Produces(typeof(Device))]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection.Get<Device>(id)
                    .ObjectOrNotFound();
            }
        }

        [HttpPost]
        public Device Post([FromBody] CreateDeviceRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                var device = connection.InsertDevice(transaction, request.Name, request.ApplicationId, request.DeviceKey);

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
                var original = connection.Get<Device>(device.Id, transaction);

                if (original == null)
                    return NotFound();

                //Check to see if we're trying to move the device to another application.
                bool applicationChanged = original.ApplicationId != device.ApplicationId;

                //We don't need to get these in call cases
                Lazy<Application> oldApplication = new Lazy<Application>(() => connection.Get<Application>(original.ApplicationId, transaction));
                Lazy<Application> newApplication = new Lazy<Application>(() => connection.Get<Application>(device.ApplicationId, transaction));

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
                    || original.AgentVersionId != device.AgentVersionId
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
                    "update Devices set " +
                    "  Name = @Name, " + 
                    "  ApplicationId = @ApplicationId, " + 
                    "  ApplicationVersionId = @ApplicationVersionId, " + 
                    "  AgentVersionId = @AgentVersionId, " +  
                    "  RootFileSystemVersionId = @RootFileSystemVersionId," + 
                    "  ConfigurationVersion = @ConfigurationVersion " +
                    "where " +
                    "  Id = @Id";

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


        /// <summary>
        /// Logically deletes the device.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                const string sql = "update Device set " +
                                   "  IsDeleted = 1 " +
                                   "where " +
                                   "  Id = @id" +
                                   "  and IsDeleted = 0 ";

                //Execute the delete
                if (connection.Execute(sql, new {id}, transaction) != 1)
                    return NotFound();

                //Log it!!!!!
                connection.InsertDeviceEvent(transaction, id, DeviceEventType.Deleted, $"Device {id:N} has been deleted (logically - not physically).");

                //We're done
                transaction.Commit();

                //Everything is good
                return Ok();
            }
        }
    }
}