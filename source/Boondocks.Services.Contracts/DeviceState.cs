namespace Boondocks.Services.Contracts
{
    public enum DeviceState
    {
        /// <summary>
        ///     The server has never heard from the device.
        /// </summary>
        New = 0,

        /// <summary>
        ///     Everything is chill.
        /// </summary>
        Idle = 1,

        /// <summary>
        ///     The unit is downloading something.
        /// </summary>
        Downloading = 2,

        /// <summary>
        ///     The device is updating something.
        /// </summary>
        Updating = 3
    }
}