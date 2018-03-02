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
    [Route("v1/agentVersions")]
    public class AgentVersionsController : Controller
    {

        private readonly IDbConnectionFactory _connectionFactory;

        public AgentVersionsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public AgentVersion[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<AgentVersion>(
                "select * from AgentVersions",
                Request.Query,
                new[]
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

        [HttpPost]
        public AgentVersion Post([FromBody] CreateAgentVersionRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                AgentVersion agentVersion = new AgentVersion()
                {
                    Name = request.Name,
                    ImageId = request.ImageId,
                    DeviceTypeId = request.DeviceTypeId,
                    Logs = request.Logs,
                }.SetNew();

                connection.Insert(agentVersion, transaction);

                transaction.Commit();

                return agentVersion;
            }
        }
    }
}