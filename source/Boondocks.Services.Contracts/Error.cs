namespace Boondocks.Services.Contracts
{
    using Newtonsoft.Json;

    public class Error
    {
        public Error()
        {
        }

        public Error(string message)
        {
            Message = message;
        }

        /// <summary>
        /// The error message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}