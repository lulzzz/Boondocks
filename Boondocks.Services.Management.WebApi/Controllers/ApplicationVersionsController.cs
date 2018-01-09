using System;
using System.Linq;
using System.Net;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.WebApi.Model;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Swashbuckle.AspNetCore.Swagger;
using SortDirection = Boondocks.Services.DataAccess.SortDirection;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/applicationVersions")]
    public class ApplicationVersionsController : Controller
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationVersionsController(IDbConnectionFactory connectionFactory, IMongoDatabase mongooDatabase)
        {
            _mongoDatabase = mongooDatabase ?? throw new ArgumentNullException(nameof(mongooDatabase));
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
        /// Upload an application container
        /// </summary>
        /// <param name="id"></param>
        /// <param name="applicationId"></param>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(typeof(ApplicationVersion))]
        [Consumes("multipart/form-data")]
        public IActionResult Post(
            [FromQuery] Guid id,
            [FromQuery] Guid applicationId,
            [FromQuery] string name,
            IFormFile file)
        {
            if (id == Guid.Empty)
                return BadRequest("A default id was specified.");

            if (file == null)
                return BadRequest("No upload file was provided.");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                Application application = connection.Get<Application>(applicationId);

                if (application == null)
                    return NotFound($"Unable to find application with id {applicationId}.");

                ApplicationVersion applicationVersion = new ApplicationVersion()
                {
                    Id = id,
                    Name = name,
                    ApplicationId = applicationId
                }.SetNew();

                var bucket = new GridFSBucket(_mongoDatabase);

                string filename = $"ApplicationVersion_{id:D}";

                using (var sourceStream = file.OpenReadStream())
                {
                    bucket.UploadFromStream(filename, sourceStream);
                }

                connection.Insert(applicationVersion);

                return Ok(applicationVersion);
            }
        }
    }

    
}