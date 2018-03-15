using System;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    using Dapper;
    using DataAccess;
    using DataAccess.Domain;
    using DataAccess.Interfaces;
    using Services.Contracts;

    [Produces("application/json")]
    [Route("v1/health")]
    public class HealthController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public HealthController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        [HttpGet]
        public GetHealthResponse Get()
        {
            return new GetHealthResponse
            {
                Items = new[]
                {
                    GetDatabaseHealth()
                }
            };
        }

        /// <summary>
        /// Wrapped up the try / catch logic for testing a health item.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private GetHealthResponseItem Test(string name, Func<string> func)
        {
            try
            {
                return new GetHealthResponseItem()
                {
                    Passed = true,
                    Name = name,
                    Message = func()
                };
            }
            catch (Exception ex)
            {
                return new GetHealthResponseItem()
                {
                    Passed = false,
                    Name = name,
                    Message = ex.Message
                };
            }
        }

        private GetHealthResponseItem GetDatabaseHealth()
        {
            return Test("database", () =>
            {
                //Talk to the database
                using (var connection = _connectionFactory.CreateAndOpen())
                {
                    //Query something so we know whether it worked or not 
                    connection.Query<DeviceType>("select * from DeviceTypes");
                }

                return "passed";
            });
        }
    }
}