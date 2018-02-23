namespace Boondocks.Agent.Logs
{
    using System;

    public class DockerLogEvent
    {
        public DockerLogEvent(DateTime timestampUtc, DateTime timestampLocal, StreamType type, string content)
        {
            TimestampUtc = timestampUtc;
            TimestampLocal = timestampLocal;
            Type = type;
            Content = content;
        }

        public DateTime TimestampUtc { get; }

        public DateTime TimestampLocal { get; }

        public StreamType Type { get; }

        public string Content { get; }
    }
}