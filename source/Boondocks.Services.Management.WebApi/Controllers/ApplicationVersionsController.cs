using System;
using System.Linq;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.Contracts;
using Boondocks.Services.Management.WebApi.Model;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;


namespace Boondocks.Services.Management.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/applicationVersions")]
    public class ApplicationVersionsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationVersionsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public ApplicationVersion[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<ApplicationVersion>(
                "select * from ApplicationVersions",
                Request.Query,
                new []
                {
                    new SortableColumn("applicationId", "ApplicationId"),
                    new SortableColumn("createdUtc", "CreatedUtc", true, SortDirection.Descending),
                });

            queryBuilder.TryAddGuidParameter("applicationId", "ApplicationId");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder.Execute(connection)
                    .ToArray();
            }
        }   

        [HttpGet("{id}")]
        [Produces(typeof(ApplicationVersion))]
        public IActionResult Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection.Get<ApplicationVersion>(id)
                    .ObjectOrNotFound();
            }
        }

        /// <summary>
        /// Upload an application version / docker image.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(typeof(ApplicationVersion))]
        public IActionResult Post([FromBody] CreateApplicationVersionRequest request)
        {
            if (request == null)
                return BadRequest("No request was specified.");

            if (request.ApplicationId == Guid.Empty)
                return BadRequest("An empty application id was specified.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("No name was specified.");

            if (string.IsNullOrWhiteSpace(request.ImageId))
                return BadRequest("No ImageId was specified.");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                //Make sure that the application exists.
                Application application = connection.Get<Application>(request.ApplicationId);

                //Make sure that we found it
                if (application == null)
                    return NotFound($"Unable to find application with id {request.ApplicationId}.");

                //Create the item
                ApplicationVersion applicationVersion = new ApplicationVersion()
                {
                    Name = request.Name,
                    ApplicationId = request.ApplicationId,
                    ImageId = request.ImageId,
                    Logs = request.Logs
                }.SetNew();

                //Insert into the relational database
                connection.Insert(applicationVersion);

                return Ok(applicationVersion);
            }
        }
    }
}