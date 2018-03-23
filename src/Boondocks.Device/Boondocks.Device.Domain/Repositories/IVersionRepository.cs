using System;
using System.Threading.Tasks;
using Boondocks.Device.Domain.Entities;

namespace Boondocks.Device.Domain.Repositories
{
    public interface IVersionRepository
    {
        Task<AgentVersion> GetAgentVersion(Guid id);
        Task<ApplicationVersion> GetApplicationVersion(Guid id);
        
    }
}