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
    [Route("api/Application")]
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
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<Application>("select * from Applications order by Name")
                    .ToArray();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .GetApplication(id)
                    .ObjectOrNotFound();
            }
        }

        [HttpPost]
        public Application Post(CreateApplicationRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                Application application = new Application()
                {
                    DeviceTypeId = request.DeviceTypeId,
                    Name = request.Name
                }.SetNew();

                connection.Execute(
                    @"insert Applications(Id, Name, DeviceTypeId, CreatedUtc) values (@Id, @Name, @DeviceTypeId, @CreatedUtc)",
                    application, transaction);

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
                var original = connection.GetApplication(application.Id);

                if (original == null)
                    return NotFound();

                //TODO: Verify the versions

                //Check to see if any of the versions changed
                bool deviceConfigurationChanged =
                    original.ApplicationVersionId != application.ApplicationVersionId
                    || original.SupervisorVersionId != application.SupervisorVersionId
                    || original.RootFileSystemVersionId != application.RootFileSystemVersionId;

                //Update the application record
                connection.Execute(
                    @"update Applications set Name = @Name, ApplicationVersionId = @ApplicationVersionId, SupervisorVersionId = @SupervisorVersionId, RootFileSystemVersionId = @RootFileSystemVersionId  where Id = @Id",
                    application,
                    transaction);

                //TODO: Update the configuration version on all of the effected devices

                //TODO: Add application events
                if (original.Name != application.Name)
                {
                    connection.InsertApplicationEvent(
                        transaction, 
                        application.Id, 
                        ApplicationEventType.Renamed, 
                        $"Application name changed from '{original.Name}' to '{application.Name}'.");
                }

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