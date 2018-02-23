namespace Boondocks.Services.Device.Contracts
{
    using Services.Contracts;

    public class SubmitApplicationLogsRequest
    {
        public bool IsFirst { get; set; }

        public DockerLogEvent[] Events { get; set;  }
    }
}