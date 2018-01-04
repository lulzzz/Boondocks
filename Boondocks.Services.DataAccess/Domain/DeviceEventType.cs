namespace Boondocks.Services.DataAccess.Domain
{
    public enum DeviceEventType
    {
        Created = 0,

        EnvironmentVariableChanged = 1,

        MovedToApplication = 2,

        Disabled = 3,

        Deleted = 4,

        ApplicationVersionChanged = 2,

        SupervisorVersionChanged = 3,

        RootFileSystemChanged = 4
    }
}