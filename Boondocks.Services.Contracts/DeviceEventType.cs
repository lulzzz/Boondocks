namespace Boondocks.Services.Contracts
{
    public enum DeviceEventType
    {
        Created = 0,

        Renamed = 1,

        Disabled = 2,

        Enabled = 3,

        /// <summary>
        /// Moved to another application.
        /// </summary>
        Moved = 4,


        EnvironmentVariableCreated = 5,

        EnvironmentVariableRemoved = 6,

        EnvironmentVariableUpdated = 7
    }
}