namespace Boondocks.Services.Device.Contracts
{
    public class EnvironmentVariable
    {
        /// <summary>
        /// The name of the environment variable.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the environment variable.
        /// </summary>
        public string Value { get; set; }
    }
}