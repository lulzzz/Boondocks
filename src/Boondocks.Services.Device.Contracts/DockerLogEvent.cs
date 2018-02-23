namespace Boondocks.Services.Contracts
{
    using System;

    public class DockerLogEvent
    {
        public DockerLogEvent()
        {
        }

        public DockerLogEvent(DateTime timestampUtc, DateTime timestampLocal, DockerLogEventType type, string content)
        {
            TimestampUtc = timestampUtc;
            TimestampLocal = timestampLocal;
            Type = type;
            Content = content;
        }

        public DateTime TimestampUtc { get; set; }

        public DateTime TimestampLocal { get; set; }

        public DockerLogEventType Type { get; set; }

        public string Content { get; set; }
    }
}