namespace Boondocks.Cli
{
    using System;
    using System.IO;

    /// <summary>
    ///     Doesn't actually create the temporary file, but makes sure that it gets deleted after use.
    /// </summary>
    internal class TemporaryFile : IDisposable
    {
        private bool _isDisposed;

        public TemporaryFile(string path = null)
        {
            Path = path ?? System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid():D}.tmp");
        }

        public string Path { get; }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (File.Exists(Path))
            {
                try
                {
                    File.Delete(Path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Unable to delete temp file: {ex.Message}");
                }
                
            }
        }
    }
}