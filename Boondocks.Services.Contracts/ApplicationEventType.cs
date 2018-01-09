namespace Boondocks.Services.Contracts
{
    public enum ApplicationEventType
    {
        Created = 0,

        Renamed = 1,

        EnvironmentVariableCreated = 5,

        EnvironmentVariableRemoved = 6,

        EnvironmentVariableUpdated = 7,
    }
}