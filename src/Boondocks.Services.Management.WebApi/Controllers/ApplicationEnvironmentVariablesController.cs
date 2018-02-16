using System;
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
    [Route("v1/applicationEnvironmentVariables")]
    public class ApplicationEnvironmentVariablesController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationEnvironmentVariablesController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Query parameters are 'applicationId'.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApplicationEnvironmentVariable[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<ApplicationEnvironmentVariable>("select * from ApplicationEnvironmentVariables", Request.Query);

            queryBuilder.TryAddGuidParameter("applicationId", "ApplicationId");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder.Execute(connection)
                    .ToArray();
            }
        }

        /// <summary>
        /// Gets a single ApplicationEnvironmentVariable given its id.
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(ApplicationEnvironmentVariable))]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Get<ApplicationEnvironmentVariable>(id)
                    .ObjectOrNotFound();
            }
        }

        /// <summary>
        /// Create an environment variable at the application level.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public ApplicationEnvironmentVariable Post([FromBody] CreateApplicationEnvironmentVariableRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                //Insert the variable
                var variable = connection.InsertApplicationEnvironmentVariable(transaction, request.ApplicationId, request.Name,
                    request.Value);

                //Insert an event
                connection.InsertApplicationEvent(
                    transaction,
                    variable.ApplicationId,
                    ApplicationEventType.EnvironmentVariableCreated,
                    $"Application  Environment variable '{variable.Name}' created with value: '{variable.Value}'.");

                //Update the configuration version
                connection.SetNewDeviceConfigurationVersionForApplication(transaction, request.ApplicationId);

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
        public IActionResult Put([FromBody] ApplicationEnvironmentVariable variable)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                //Get the original so we can log the delta.
                ApplicationEnvironmentVariable original = connection.Get<ApplicationEnvironmentVariable>(variable.Id, transaction);

                if (original == null)
                    return NotFound();

                const string sql = " update ApplicationEnvironmentVariables set" +
                                   "  Name = @Name," +
                                   "  Value = @Value " +
                                   "where" +
                                   "  Id = @Id";

                //Update it (and ensure that it happened)
                if (connection.Execute(sql, variable, transaction) != 1)
                    return NotFound();

                //Insert an event
                connection.InsertApplicationEvent(
                    transaction,
                    variable.ApplicationId,
                    ApplicationEventType.EnvironmentVariableUpdated,
                    $"Application Environment variable changed from ({original.Name}/{original.Value}) to ({variable.Name}/{variable.Value}).");

                //Update the configuration version
                connection.SetNewDeviceConfigurationVersionForApplication(transaction, original.ApplicationId);

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
                ApplicationEnvironmentVariable original = connection.Get<ApplicationEnvironmentVariable>(id, transaction);

                if (original == null)
                    return NotFound();

                const string deleteSql = "delete from ApplicationEnvironmentVariables where Id = @Id";

                //Delete it!
                if (connection.Execute(deleteSql, new { Id = id }, transaction) != 1)
                    return NotFound();

                //Insert an event
                connection.InsertApplicationEvent(
                    transaction,
                    original.ApplicationId,
                    ApplicationEventType.EnvironmentVariableUpdated,
                    $"Application Environment variable '{original.Name}' deleted.");

                //Update the configuration version
                connection.SetNewDeviceConfigurationVersionForApplication(transaction, original.ApplicationId);

                //We're done here.
                transaction.Commit();
            }

            return Ok();
        }
    }
}