using System;
using Boondocks.Device.Api.Models;
using NetFusion.Messaging.Types;

namespace Boondocks.Device.Api.Queries
{
    public class ApplicationImageInfo : Query<ImageDownloadModel>
    {
        public Guid Id { get; }

        public ApplicationImageInfo(Guid id)
        {
            Id = id;
        }
    }
}