using System.Threading.Tasks;
using NetFusion.Messaging;
using Boondocks.Device.Api.Commands;
using Boondocks.Base.Data;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Repositories;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Api.Models;

namespace Boondocks.Device.App.Ports
{
    /// <summary>
    /// Port contianing log event related handlers.
    /// </summary>
    public class LogEventPort : IMessageConsumer
    {
        private readonly IRepositoryContext<DeviceDb> _repoContext;
        private readonly IDeviceRepository _deviceRepository;
        
        public LogEventPort(
            IRepositoryContext<DeviceDb> repoContext,
            IDeviceRepository deviceRepository)
        {
            _repoContext = repoContext;
            _deviceRepository = deviceRepository;
        }

        [InProcessHandler]
        public async Task When (LogEventReceived command)
        {
            // Create a new device log domain entity from received command.
            var deviceLog = DeviceLog.ForExistingDevice(command.DeviceId);

            foreach (LogEventModel model in command.LogEvents)
            {
                var logEvent = new ApplicationLog {
                    Type = model.Type,
                    Message = model.Content,
                    CreatedLocal = model.TimestampLocal,
                    CreatedUtc = model.TimestampUtc
                };

                deviceLog.AddLog(logEvent);
            }

            // Save the device log and optionally delete any prior entries.
            using (_repoContext)
            {
                await _deviceRepository.WriteDeviceLog(deviceLog, command.PurgeExisting);
            }
        }
    }
}