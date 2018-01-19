using System;

namespace Boondocks.Services.DataAccess.Domain
{
    public class ApplicationEnvironmentVariable : EnvironmentVariable
    {
        public Guid ApplicationId { get; set; }
    }
}