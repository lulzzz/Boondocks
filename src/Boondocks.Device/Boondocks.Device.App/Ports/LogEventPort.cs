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
        private readonly IDeviceContext _deviceContext;
        private readonly IRepositoryContext<DeviceDb> _repoContext;
        private readonly IApplicationLogRepository _applicationLogRepo;
        
        public LogEventPort(
            IDeviceContext deviceContext,
            IRepositoryContext<DeviceDb> repoContext,
            IApplicationLogRepository applicationLogRepo)
        {
            _deviceContext = deviceContext;
            _repoContext = repoContext;
            _applicationLogRepo = applicationLogRepo;
        }

        [InProcessHandler]
        public async Task When (LogEventReceived command)
        {
            // Create a new device log domain entity from received command.
            var deviceLog = DeviceLog.ForExistingDevice(_deviceContext.DeviceId);

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
                await _applicationLogRepo.WriteDeviceLog(deviceLog, command.PurgeExisting);
            }
        }
    }
}