namespace Boondocks.Agent.Base.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRootFileSystemUpdateService
    {
        Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}