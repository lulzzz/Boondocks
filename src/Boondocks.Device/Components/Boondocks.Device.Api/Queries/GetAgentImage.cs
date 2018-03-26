using System;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;

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