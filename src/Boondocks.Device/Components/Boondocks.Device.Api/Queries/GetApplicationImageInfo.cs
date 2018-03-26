using System;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;

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