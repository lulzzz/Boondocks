using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    using Common;
    using Contracts;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using DataAccess.Domain;
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Logging;

    [Authorize]
    [Produces("application/json")]
    [Route("v1/applicationLogs")]
    public class ApplicationLogsController : DeviceControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ApplicationLogsController> _logger;

        public ApplicationLogsController(
            IDbConnectionFactory connectionFactory,
            ILogger<ApplicationLogsController> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public void Post([FromBody]SubmitApplicationLogsRequest request)
        {
            using (var connection = _connectionFactory.Create())
            using (var transaction = connection.BeginTransaction())
            {
                //If this is the first batch, we'll go ahead and kill the existing entries.
                if (request.IsFirst)
                {
                    const string deleteSql = "delete from ApplicationLogs where DeviceId = @deviceId";

                    connection.Execute(deleteSql, new {deviceId = DeviceId}, transaction);
                }

                //Transform the request into something we can insert.
                var entries = request.Events
                    .Select(e => new ApplicationLog
                    {
                        Id = Guid.NewGuid(),
                        Message = e.Content,
                        DeviceId = DeviceId,
                        CreatedLocal = e.TimestampLocal,
                        CreatedUtc = e.TimestampUtc
                    });

                //Insert these guys
                connection.Insert(entries, transaction);

                transaction.Commit();
            }
        }
    }
}