using System;

namespace Boondocks.Services.Management.WebApi.Model
{
    public static class RepositoryNameFactory
    {
        public static string Create(Guid applicationId)
        {
            return $"{applicationId:D}";
        }
    }
}