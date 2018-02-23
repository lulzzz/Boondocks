namespace Boondocks.Services.Contracts
{
    public enum DockerLogEventType : byte
    {
        Stdin = 0,

        Stdout = 1,

        Stderr = 2,
    }
}