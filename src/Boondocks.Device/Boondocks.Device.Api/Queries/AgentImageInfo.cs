using System;
using Boondocks.Device.Api.Models;
using NetFusion.Messaging.Types;

namespace Boondocks.Device.Api.Queries
{
    public class AgentImageInfo : Query<ImageDownloadModel>
    {
        public Guid Id { get; }

        public AgentImageInfo(Guid id)
        {
            Id = id;
        }
    }   
}