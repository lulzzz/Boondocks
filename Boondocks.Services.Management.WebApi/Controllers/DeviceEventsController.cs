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
    [Route("api/DeviceEvents")]
    public class DeviceEventsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceEventsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public DeviceEvent[] Get()
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<DeviceEvent>("select * from DeviceEvents order by CreatedUtc desc")
                    .ToArray();
            }
        }
    }
}