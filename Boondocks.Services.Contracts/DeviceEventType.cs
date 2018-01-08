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

        EnvironmentVariableUpdated = 7,

        /// <summary>
        /// The device has been logically deleted.
        /// </summary>
        Deleted = 8,

        /// <summary>
        /// The device has been brought back from the dead (logical deletion).
        /// </summary>
        Resurrected = 9

          
    }
}