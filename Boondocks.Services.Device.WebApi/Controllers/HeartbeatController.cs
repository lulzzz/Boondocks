using System;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApi.Common;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/Heartbeat")]
    [Authorize]
    public class HeartbeatController : DeviceControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public HeartbeatController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// A heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HeartbeatResponse Post(HeartbeatRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                const string sql = "select ConfigurationVersion from Devices where Id = @Id";

                //Get the response
                var response = connection.QuerySingle<HeartbeatResponse>(sql, new { Id = DeviceId}  );

                return response;
            }   
        }
    }
}