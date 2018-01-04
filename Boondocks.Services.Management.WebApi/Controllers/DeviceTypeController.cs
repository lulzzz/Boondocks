using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Management.Contracts;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Management.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/DeviceType")]
    public class DeviceTypeController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceTypeController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public DeviceType[] Get()
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<DeviceType>("select Id, Name, CreatedUtc from DeviceTypes order by Name")
                    .ToArray();               
            }
        }

        [HttpGet("{id}")]
        public DeviceType Get(Guid id)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<DeviceType>("select Id, Name, CreatedUtc from DeviceTypes where Id = @Id order by Name",
                        new { Id = id })
                    .First();
            }
        }
    }
}