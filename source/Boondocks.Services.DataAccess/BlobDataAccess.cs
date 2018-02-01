namespace Boondocks.Services.DataAccess
{
    using System;
    using System.IO;
    using Interfaces;
    using MongoDB.Driver.GridFS;

    public class BlobDataAccess : IBlobDataAccess
    {
        private readonly IGridFSBucket _bucket;
        private readonly Func<Guid, string> _filenameFactory;

        public BlobDataAccess(IGridFSBucket bucket, Func<Guid, string> filenameFactory)
        {
            _filenameFactory = filenameFactory;
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        }

        public void UploadFromStream(Guid id, Stream sourceStream)
        {
            var filename = GetFilename(id);

            _bucket.UploadFromStream(filename, sourceStream);
        }

        public void DownloadToStream(Guid id, Stream targetStream)
        {
            var filename = GetFilename(id);

            _bucket.DownloadToStreamByName(filename, targetStream);
        }

        public Stream GetDownloadStream(Guid id)
        {
            var filename = GetFilename(id);

            return _bucket.OpenDownloadStreamByName(filename);
        }

        public string GetFilename(Guid id)
        {
            return _filenameFactory(id);
        }
    }
}