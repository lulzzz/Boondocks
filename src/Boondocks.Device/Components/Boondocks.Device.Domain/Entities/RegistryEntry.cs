using Boondocks.Device.Domain.Entities;

namespace Boondocks.Device.Domain
{
    public class RegistryEntry
    {
        public string RegistryName { get; set; }
        public IVersionReference ApplicationVersion { get; set; }
        public IVersionReference AgentVersion { get;  set; }

    }
}