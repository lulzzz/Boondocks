using Boondocks.Device.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Boondocks.Device.Domain.Repositories
{
    public interface IApplicationRepository
    {
        Task<ApplicationEntity> GetApplication(Guid deviceId);
        Task<ApplicationVersion> GetApplicationVersion(Guid id);
    }
}