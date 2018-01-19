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
        private readonly IBlobDataAccessProvider _blobDataAccessProvider;
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationVersionsController(IDbConnectionFactory connectionFactory, IBlobDataAccessProvider blobDataAccessProvider)
        {
            _blobDataAccessProvider = blobDataAccessProvider ?? throw new ArgumentNullException(nameof(blobDataAccessProvider));
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
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(typeof(ApplicationVersion))]
        [Consumes("multipart/form-data")]
        public IActionResult Post(IFormFile file)
        {
            // Found some info here on dealing with multiple parts of a request.
            // I couldn't figure out how to make request a parameter. [FromForm] didn't exactly work. Dangit.
            // https://forums.asp.net/t/2099194.aspx?Net+Core+Web+API+How+to+upload+multi+part+form+data

            var requestJson = Request.Form["request"];

            //Make sure we got some json
            if (string.IsNullOrWhiteSpace(requestJson))
                return BadRequest("No request object was specified.");

            //Deserialize the request
            CreateApplicationVersionRequest request =
                Newtonsoft.Json.JsonConvert.DeserializeObject<CreateApplicationVersionRequest>(requestJson);

            if (request == null)
                return BadRequest("No request was specified.");

            if (request.ApplicationId == Guid.Empty)
                return BadRequest("An empty application id was specified.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("No name was specified.");

            if (string.IsNullOrWhiteSpace(request.ImageId))
                return BadRequest("No ImageId was specified.");

            if (file == null)
                return BadRequest("No upload file was provided.");

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
                    ImageId = request.ImageId
                }.SetNew();

                //Store this mammer jammer in a blob
                using (var sourceStream = file.OpenReadStream())
                {
                    _blobDataAccessProvider.ApplicationVersionImages.UploadFromStream(applicationVersion.Id, sourceStream);
                }

                //Insert into the relational database
                connection.Insert(applicationVersion);

                return Ok(applicationVersion);
            }
        }
    }
}