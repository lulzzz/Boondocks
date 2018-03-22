using System.Threading.Tasks;
using Boondocks.Device.Api.Commands;
using Boondocks.Device.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;

namespace Boondocks.Device.WebApi.Controllers
{
    /// <summary>
    /// Controller for recording device logs.
    /// </summary>
    [Route("api/v1/boondocks/device/logs")]
    public class LogEventController : Controller
    {
        private readonly IMessagingService _messagingSrv;

        public LogEventController(IMessagingService messagingSrv)
        {
            _messagingSrv = messagingSrv;
        }

        /// <summary>
        /// Allows current requesting device to record a set of logs.
        /// </summary>
        /// <param name="logEvents">The logs to be saved.</param>
        [HttpPost]
        public Task RecordLogEvent([FromBody]LogEventsModel logEvents)
        {
            var command = LogEventReceived.HavingDetails(logEvents);
            return _messagingSrv.SendAsync(command);
        }

        /// <summary>
        /// Allows current requesting device to clear all of its existing logs and
        /// record a new set of logs.
        /// </summary>
        /// <param name="logEvents">The received log events.</param>
        [HttpPost("purge")]
        public Task PurgeAndRecordLogEvent([FromBody]LogEventsModel logEvents)
        {
            var command = LogEventReceived.Purge();

            if (logEvents != null)
            {
                command = LogEventReceived.HavingDetails(logEvents, purgeExisting: true);
            }

            return _messagingSrv.SendAsync(command);
        }
    }
}