using System;
using System.Threading.Tasks;
using Boondocks.Device.Domain.Entities;

namespace Boondocks.Device.Domain.Repositories
{
    public interface IApplicationRepository
    {
        Task<ApplicationEntity> GetApplication(Guid deviceId);
        Task<ApplicationVersion> GetApplicationVersion(Guid id);
    }
}