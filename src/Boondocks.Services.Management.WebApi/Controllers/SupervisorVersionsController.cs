using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using Contracts;
    using Dapper.Contrib.Extensions;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;

    [Produces("application/json")]
    [Route("v1/supervisorVersions")]
    public class SupervisorVersionsController : Controller
    {

        private readonly IDbConnectionFactory _connectionFactory;

        public SupervisorVersionsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public SupervisorVersion[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<SupervisorVersion>(
                "select * from SupervisorVersions",
                Request.Query,
                new[]
                {
                    new SortableColumn("name", "Name", true),
                    new SortableColumn("createdUtc", "CreatedUtc"),
                });

            queryBuilder.TryAddGuidParameter("deviceArchitectureId", "DeviceArchitectureId");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder.Execute(connection)
                    .ToArray();
            }
        }

        [HttpPost]
        public SupervisorVersion Post([FromBody] CreateSupervisorVersionRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                SupervisorVersion supervisorVersion = new SupervisorVersion()
                {
                    Name = request.Name,
                    ImageId = request.ImageId,
                    DeviceArchitectureId = request.DeviceArchitectureId,
                    Logs = request.Logs,
                }.SetNew();

                connection.Insert(supervisorVersion, transaction);

                transaction.Commit();

                return supervisorVersion;
            }
        }
    }
}