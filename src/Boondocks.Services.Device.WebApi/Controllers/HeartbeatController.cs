namespace Boondocks.Services.Device.WebApi.Controllers
{
    using System;
    using Common;
    using Contracts;
    using Dapper;
    using DataAccess;
    using DataAccess.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    [Produces("application/json")]
    [Route("v1/heartbeat")]
    public class HeartbeatController : DeviceControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<HeartbeatController> _logger;

        public HeartbeatController(IDbConnectionFactory connectionFactory, ILogger<HeartbeatController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        /// <summary>
        /// Proceses the device heartbeat so we know that the device is still alive. We also send back the 
        /// configuration version of the device so that the device knows if it needs to update itself.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HeartbeatResponse Post([FromBody] HeartbeatRequest request)
        {
            using (var connection = _connectionFactory.CreateAndOpen())
            {
                const string updateSql = "update DeviceStatus " +
                                         "set " +
                                         "  AgentVersion = @AgentVersion, " +
                                         "  ApplicationVersion = @ApplicationVersion, " +
                                         "  UptimeSeconds = @UptimeSeconds, " +
                                         "  LastContactUtc = @Utc, " +
                                         "  State = @State " +
                                         "where " +
                                         "  DeviceId = @DeviceId";

                connection.Execute(updateSql, new
                {
                    request.UptimeSeconds,
                    Utc = DateTime.UtcNow,
                    DeviceId,
                    request.State,
                    request.AgentVersion,
                    request.ApplicationVersion
                });

                const string responseSql = "select ConfigurationVersion from Devices where Id = @Id";

                //Get the response
                var response = connection.QuerySingle<HeartbeatResponse>(responseSql, new {Id = DeviceId});

                _logger.LogTrace(
                    "Heartbeat receved for device {DeviceId}. Device has been up for {UptimeSeconds} seconds and is using ConfigurationVersion {ConfigurationVersion} sent.",
                    DeviceId, request.UptimeSeconds, response.ConfigurationVersion);

                return response;
            }
        }
    }
}