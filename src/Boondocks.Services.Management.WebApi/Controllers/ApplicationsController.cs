﻿
namespace Boondocks.Services.Management.WebApi.Controllers
{
    using System;
    using System.Linq;
    using DataAccess;
    using DataAccess.Interfaces;
    using Contracts;
    using Model;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using Microsoft.AspNetCore.Mvc;

    using DataAccess.Domain;

    [Produces("application/json")]
    [Route("v1/applications")]
    public class ApplicationsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public Application[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<Application>(
                "select * from Applications", 
                Request.Query,
                new []
                {
                    new SortableColumn("name", "Name", true), 
                    new SortableColumn("createdUtc", "CreatedUtc"), 
                });

            queryBuilder.TryAddGuidParameter("deviceTypeId", "DeviceTypeId");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder.Execute(connection)
                    .ToArray();
            }
        }

        [HttpGet("{id}")]
        [Produces(typeof(Application))]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection.Get<Application>(id, null)
                    .ObjectOrNotFound();
            }
        }

        [HttpPost]
        public Application Post([FromBody]CreateApplicationRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                Application application = new Application()
                {
                    DeviceTypeId = request.DeviceTypeId,
                    Name = request.Name
                }.SetNew();

                connection.Insert(application, transaction);

                connection.InsertApplicationEvent(
                    transaction, 
                    application.Id, 
                    ApplicationEventType.Created, 
                    $"Application '{application.Name}' created.");

                transaction.Commit();

                return application;
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] Application application)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                var original = connection.Get<Application>(application.Id, transaction);

                if (original == null)
                    return NotFound();

                //TODO: Verify the versions

                //Check to see if any of the versions changed
                bool deviceConfigurationChanged =
                    original.ApplicationVersionId != application.ApplicationVersionId
                    || original.AgentVersionId != application.AgentVersionId
                    || original.RootFileSystemVersionId != application.RootFileSystemVersionId;

                const string sql =
                    @"update Applications set " +
                    "  Name = @Name, " +
                    "  ApplicationVersionId = @ApplicationVersionId, " +
                    "  AgentVersionId = @AgentVersionId, " +
                    "  RootFileSystemVersionId = @RootFileSystemVersionId  " +
                    "where" +
                    "  Id = @Id";

                //Update the application record
                connection.Execute(sql, application, transaction);

                //Did we update anything that affects the devices?
                if (deviceConfigurationChanged)
                {
                    // Update the configuration version on all of the effected devices
                    connection.SetNewDeviceConfigurationVersionForApplication(transaction, application.Id);
                }

                //Name changed event
                if (original.Name != application.Name)
                {
                    connection.InsertApplicationEvent(
                        transaction, 
                        application.Id, 
                        ApplicationEventType.Renamed, 
                        $"Application name changed from '{original.Name}' to '{application.Name}'.");
                }

                //TODO: Add other application events

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
                //Note that we're counting on the database to prevent deleting anything where there are devices.
                int result = connection.Execute("delete from Applications where Id = @id", new { id });

                if (result == 0)
                {
                    return NotFound();
                }
            }

            return Ok();
        }
    }
}