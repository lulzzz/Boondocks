namespace Boondocks.Agent.Logs
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class LogStreamReader : IDisposable
    {
        private readonly byte[] _header = new byte[8];
        private readonly Stream _stream;
        private int _remaining;

        public LogStreamReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public async Task<DockerLogEvent> ReadAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            while (_remaining == 0)
            {
                for (var i = 0; i < _header.Length;)
                {
                    var n = await _stream.ReadAsync(_header, i, _header.Length - i, cancellationToken).ConfigureAwait(false);
                    if (n == 0)
                    {
                        if (i == 0)
                        {
                            // End of the stream.
                            return null;
                        }

                        throw new EndOfStreamException();
                    }

                    i += n;
                }

                if ((uint)_header[0] > 2U)
                    throw new IOException("unknown stream type");

                _remaining = (_header[4] << 24) |
                             (_header[5] << 16) |
                             (_header[6] << 8) |
                             _header[7];
            }


            byte[] buffer = new byte[_remaining];

            while (_remaining > 0)
            {
                var read = await _stream.ReadAsync(buffer, buffer.Length - _remaining, _remaining, cancellationToken).ConfigureAwait(false);

                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                _remaining -= read;
            }

            //Get the line
            string line = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            //Trim the newline at the end
            if (line.EndsWith("\n"))
            {
                line = line.Substring(0, line.Length - 1);
            }

            //Get the timestamp part of the string
            string timestampString = line.Substring(0, 30);

            //Get the timestamp utc
            DateTime timestampLocal = DateTime.Parse(timestampString);
            DateTime timestampUtc = timestampLocal.ToUniversalTime();

            return new DockerLogEvent(timestampUtc, timestampLocal, (StreamType)_header[0], line.Substring(30));
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}