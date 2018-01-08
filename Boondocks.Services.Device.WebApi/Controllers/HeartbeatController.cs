using System;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApi.Common;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/Heartbeat")]
    [Authorize]
    public class HeartbeatController : DeviceControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;
        private readonly IDbConnectionFactory _connectionFactory;

        public HeartbeatController(IDbConnectionFactory connectionFactory, ILogger<HeartbeatController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// A heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HeartbeatResponse Post([FromBody]HeartbeatRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                const string updateSql = "update DeviceStatus " +
                                         "set " +
                                         "  UptimeSeconds = @UptimeSeconds, " +
                                         "  LastContactUtc = @Utc " +
                                         "where " +
                                         "  DeviceId = @DeviceId";


                connection.Execute(updateSql, new
                {
                    UptimeSeconds = request.UptimeSeconds,
                    Utc = DateTime.UtcNow,
                    DeviceId = DeviceId
                });

                const string responseSql = "select ConfigurationVersion from Devices where Id = @Id";

                //Get the response
                var response = connection.QuerySingle<HeartbeatResponse>(responseSql, new { Id = DeviceId}  );

                _logger.LogTrace("Heartbeat receved for device {DeviceId}. Device has been up for {UptimeSeconds} seconds and is using ConfigurationVersion {ConfigurationVersion} sent.", DeviceId, request.UptimeSeconds, response.ConfigurationVersion);

                return response;
            }   
        }
    }
}