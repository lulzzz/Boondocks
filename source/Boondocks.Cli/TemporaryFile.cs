using System;
using System.IO;

namespace Boondocks.Cli
{
    /// <summary>
    /// Doesn't actually create the temporary file, but makes sure that it gets deleted after use.
    /// </summary>
    internal class TemporaryFile : IDisposable
    {
        public TemporaryFile(string path = null)
        {
            Path = path ?? System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid():D}.tmp");
        }

        private bool _isDisposed;

        public string Path { get; }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }
    }
}