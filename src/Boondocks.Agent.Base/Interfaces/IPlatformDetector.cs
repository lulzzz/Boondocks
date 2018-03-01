namespace Boondocks.Agent.Base.Interfaces
{
    /// <summary>
    ///     Detects if we're executing on Linux.
    /// </summary>
    public interface IPlatformDetector
    {
        bool IsLinux { get; }
    }
}