namespace Boondocks.Agent.Base.Interfaces
{
    using System.Threading.Tasks;

    public interface IRootFileSystemVersionProvider
    {
        Task<string> GetCurrentVersionAsync();
    }
}