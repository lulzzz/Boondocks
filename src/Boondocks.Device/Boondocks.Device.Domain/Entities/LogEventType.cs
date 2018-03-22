namespace Boondocks.Device.Domain.Entities
{
    public enum LogEventType : byte
    {
        Stdin = 0,

        Stdout = 1,

        Stderr = 2,
    }
}