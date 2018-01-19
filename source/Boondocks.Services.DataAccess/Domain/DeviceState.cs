namespace Boondocks.Services.DataAccess.Domain
{
    public enum DeviceState
    {
        /// <summary>
        /// The device has never contacted the server.
        /// </summary>
        None = 0,

        /// <summary>
        /// Normal
        /// </summary>
        Idle = 1,

        /// <summary>
        /// Downloading some sort of image
        /// </summary>
        Downloading = 2,

        /// <summary>
        /// The device is in the process of updating
        /// </summary>
        Updating = 3,
    }
}