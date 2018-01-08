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
    [Route("api/DeviceEnvironmentVariables")]
    public class DeviceEnvironmentVariablesController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceEnvironmentVariablesController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Query parameters are 'deviceId'.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DeviceEnvironmentVariable[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<DeviceEnvironmentVariable>("select * from DeviceEnvironmentVariables", Request.Query);

            queryBuilder.TryAddGuidParameter("deviceId", "DeviceId");
            

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder.Execute(connection)
                    .ToArray();
            }
        }

        /// <summary>
        /// Gets a DeviceEnvironmentVariable by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .GetDeviceEnvironmentVariable(id)
                    .ObjectOrNotFound();
            }
        }

        /// <summary>
        /// Create an environment variable.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public DeviceEnvironmentVariable Post([FromBody] CreateDeviceEnvironmentVariableRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                //Insert the variable
                var variable = connection.InsertDeviceEnvironmentVariable(transaction, request.DeviceId, request.Name,
                    request.Value);

                //Insert an event
                connection.InsertDeviceEvent(
                    transaction,
                    variable.DeviceId,
                    DeviceEventType.EnvironmentVariableCreated,
                    $"Device {request.DeviceId:D} Environment variable '{variable.Name}' created with value: '{variable.Value}'.");

                //Update the configuration version
                connection.SetNewDeviceConfigurationVersionForDevice(transaction, request.DeviceId);

                //We're done
                transaction.Commit();

                return variable;
            }
        }

        /// <summary>
        /// Updates an environment variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Put([FromBody] DeviceEnvironmentVariable variable)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                //Get the original so we can log the delta.
                DeviceEnvironmentVariable original = connection.GetDeviceEnvironmentVariable(variable.Id, transaction);

                if (original == null)
                    return NotFound();

                const string sql = " update DeviceEnvironmentVariables set" +
                                   "  Name = @Name," +
                                   "  Value = @Value " +
                                   "where" +
                                   "  Id = @Id";

                //Update it (and ensure that it happened)
                if (connection.Execute(sql, variable, transaction) != 1)
                    return NotFound();

                //Insert an event
                connection.InsertDeviceEvent(
                    transaction, 
                    variable.DeviceId,
                    DeviceEventType.EnvironmentVariableUpdated,
                    $"Device {original.DeviceId:D} Environment variable changed from ({original.Name}/{original.Value}) to ({variable.Name}/{variable.Value}).");

                //Update the configuration version
                connection.SetNewDeviceConfigurationVersionForDevice(transaction, original.DeviceId);

                //We're done
                transaction.Commit();
            }

            return Ok();
        }

        /// <summary>
        /// Deletes an environment variable.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                //Get the device variable (so we can log the using the DeviceId)
                DeviceEnvironmentVariable original = connection.GetDeviceEnvironmentVariable(id, transaction);

                if (original == null)
                    return NotFound();

                const string deleteSql = "delete from DeviceEnvironmentVariables where Id = @Id";

                //Delete it!
                if (connection.Execute(deleteSql, new {Id = id}, transaction) != 1)
                    return NotFound();

                //Insert an event
                connection.InsertDeviceEvent(
                    transaction,
                    original.DeviceId,
                    DeviceEventType.EnvironmentVariableUpdated,
                    $"Device {original.DeviceId:D} Environment variable '{original.Name}' deleted.");

                //Update the configuration version
                connection.SetNewDeviceConfigurationVersionForDevice(transaction, original.DeviceId);

                //We're done here.
                transaction.Commit();
            }

            return Ok();
        }
    }
}