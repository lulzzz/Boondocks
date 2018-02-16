namespace Boondocks.Services.DataAccess.Domain
{
    public enum ApplicationEventType
    {
        Created = 0,

        Renamed = 1,

        EnvironmentVariableCreated = 5,

        EnvironmentVariableRemoved = 6,

        EnvironmentVariableUpdated = 7
    }
}