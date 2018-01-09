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
    [Route("v1/applicationVersions")]
    public class ApplicationVersionsController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApplicationVersionsController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public ApplicationVersion[] Get()
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                return connection
                    .Query<ApplicationVersion>("select * from Applications order by Name")
                    .ToArray();
            }
        }   
    }
}