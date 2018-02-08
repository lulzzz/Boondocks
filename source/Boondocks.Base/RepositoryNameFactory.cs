namespace Boondocks.Base
{
    using System;

    /// <summary>
    /// Creates the canonical docker repository name for a given application.
    /// </summary>
    public class RepositoryNameFactory
    {
        public string FromApplication(Guid applicationId)
        {
            return $"{applicationId:D}";
        }
    }
}