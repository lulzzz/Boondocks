using Boondocks.Base.Auth;
using Boondocks.Device.Api.Commands;
using Boondocks.Device.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;
using NetFusion.Web.Mvc.Metadata;
using System.Threading.Tasks;

namespace Boondocks.Device.WebApi.Controllers
{
    /// <summary>
    /// Controller for recording and purging device logs.
    /// </summary>
    [Route("v1.0/device/logs"),
        GroupMeta(nameof(DeviceConfigurationController))]
    public class LogEventController : Controller
    {
        private readonly IDeviceContext _context;
        private readonly IMessagingService _messagingSrv;

        public LogEventController(
            IDeviceContext context,
            IMessagingService messagingSrv)
        {
            _context = context;
            _messagingSrv = messagingSrv;
        }

        /// <summary>
        /// Allows current requesting device to record a set of logs.
        /// </summary>
        /// <param name="logEvents">The logs to be saved.</param>
        [HttpPost,
            ActionMeta(nameof(RecordLogEvent))]
        public Task RecordLogEvent([FromBody]LogEventsModel logEvents)
        {
            var command = LogEventReceived.HavingDetails(_context.DeviceId, logEvents);
            return _messagingSrv.SendAsync(command);
        }

        /// <summary>
        /// Allows current requesting device to clear all of its existing logs and
        /// record a new set of logs.
        /// </summary>
        /// <param name="logEvents">The received log events.</param>
        [HttpPost("purge"),
             ActionMeta(nameof(PurgeAndRecordLogEvent))]
        public Task PurgeAndRecordLogEvent([FromBody]LogEventsModel logEvents)
        {
            var command = LogEventReceived.Purge(_context.DeviceId);

            if (logEvents != null)
            {
                command = LogEventReceived.HavingDetails(
                    _context.DeviceId, 
                    logEvents, 
                    purgeExisting: true);
            }

            return _messagingSrv.SendAsync(command);
        }
    }
}