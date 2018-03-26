using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;
using System;

namespace Boondocks.Device.Api.Queries
{
    public class GetAgentImageInfo : Query<IVersionReference>
    {
        public Guid Id { get; }

        public GetAgentImageInfo(Guid id)
        {
            Id = id;
        }
    }   
}