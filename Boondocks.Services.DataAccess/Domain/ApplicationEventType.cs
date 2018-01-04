namespace Boondocks.Services.DataAccess.Domain
{
    public enum ApplicationEventType
    {
        Created = 0,

        EnvironmentVariableChanged = 1,

        Renamed = 2,

        ApplicationVersionChanged = 2,

        SupervisorVersionChanged = 3
    }
}