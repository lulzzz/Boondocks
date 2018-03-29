using System;
using NetFusion.Rest.Client.Resources;

namespace Boondocks.Device.Client.Resource
{
    public class AgentVersionResource : HalResource
    {
        public Guid Id { get; set; }
        public string Registry { get; set; }
        public string RepositoryName { get; set; }
        public string ImageId { get; set; }
        public string Name { get; set; }
    }
}