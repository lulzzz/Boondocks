using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    using Contracts;
    using Dapper.Contrib.Extensions;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;

    [Produces("application/json")]
    [Route("v1/deviceArchitectures")]
    public class DeviceArchitecturesController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceArchitecturesController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public DeviceArchitecture[] Get()
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection.GetAll<DeviceArchitecture>()
                    .ToArray();
            }
        }

        [HttpPost]
        [Produces(typeof(DeviceArchitecture))]
        public IActionResult Post([FromBody] CreateDeviceArchitectureRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            using (var transaction = connection.BeginTransaction())
            {
                DeviceArchitecture deviceArchitecture = new DeviceArchitecture()
                {
                    Name = request.Name,
                }.SetNew();

                connection.Insert(deviceArchitecture, transaction);

                transaction.Commit();

                return Ok(deviceArchitecture);
            }
        }
    }
}