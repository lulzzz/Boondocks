using Boondocks.Device.Domain.Entities;

namespace Boondocks.Device.Domain
{
    public class RegistryEntry
    {
        public string Registry { get; set; }
        public IVersionReference ApplicationVersion { get; set; }
        public IVersionReference AgentVersion { get;  set; }

    }
}