using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;
using System;

namespace Boondocks.Device.Api.Queries
{
    public class GetApplicationImageInfo : Query<IVersionReference>
    {
        public Guid Id { get; }

        public GetApplicationImageInfo(Guid id)
        {
            Id = id;
        }
    }
}