namespace Boondocks.Agent.Interfaces
{
    /// <summary>
    ///     Detects if we're executing on Linux.
    /// </summary>
    internal interface IPlatformDetector
    {
        bool IsLinux { get; }
    }
}