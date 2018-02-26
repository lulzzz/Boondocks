

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Dapper;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;

    [Produces("application/json")]
    [Route("v1/applicationLogs")]
    public class ApplicationLogsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationLogsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        [Produces(typeof(ApplicationLog[]))]
        public IActionResult Get([FromQuery] Guid deviceId)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                const string sql = "select * from [ApplicationLogs] where deviceId = @deviceId order by CreatedUtc";

                var entities = connection.Query<ApplicationLog>(sql, new {deviceId});

                return Ok(entities);
            }
        }
    }
}