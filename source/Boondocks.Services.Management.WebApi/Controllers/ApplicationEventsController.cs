using System;
using System.Linq;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using DataAccess.Domain;

    [Produces("application/json")]
    [Route("v1/applicationEvents")]
    public class ApplicationEventsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationEventsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Available query parameters are deviceId, eventType.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApplicationEvent[] Get()
        {
            var queryBuilder = new SelectQueryBuilder<ApplicationEvent>(
                "select * from ApplicationEvents",
                Request.Query,
                new []
                {
                    new SortableColumn("applicationId", "ApplicationId"),
                    new SortableColumn("eventType", "EventType"),
                    new SortableColumn("createdUtc", "CreatedUtc", true, SortDirection.Descending),
                });

            queryBuilder.TryAddGuidParameter("applicationId", "ApplicationId");
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