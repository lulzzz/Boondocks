namespace Boondocks.Agent.Base.Model
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;

    public class RootFileSystemVersionProvider : IRootFileSystemVersionProvider
    {
        public Task<string> GetCurrentVersionAsync()
        {
            return Task.FromResult(Environment.OSVersion.VersionString);
        }
    }
}