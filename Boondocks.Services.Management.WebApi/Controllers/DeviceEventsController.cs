using System;
using System.Linq;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/deviceEvents")]
    public class DeviceEventsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceEventsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Available query parameters are deviceId, eventType.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DeviceEvent[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<DeviceEvent>("select * from DeviceEvents", Request.Query);

            queryBuilder.TryAddGuidParameter("deviceId", "DeviceId");
            queryBuilder.TryAddIntParameter("eventType", "EventType");

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return queryBuilder
                    .Execute(connection)
                    .ToArray();
            }
        }
    }
}