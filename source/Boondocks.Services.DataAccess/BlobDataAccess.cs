using System;
using System.IO;
using Boondocks.Services.DataAccess.Interfaces;
using MongoDB.Driver.GridFS;

namespace Boondocks.Services.DataAccess
{
    public class BlobDataAccess : IBlobDataAccess
    {
        private readonly Func<Guid, string> _filenameFactory;
        private readonly IGridFSBucket _bucket;

        public BlobDataAccess(IGridFSBucket bucket, Func<Guid, string> filenameFactory)
        {
            _filenameFactory = filenameFactory;
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        }

        public void UploadFromStream(Guid id, Stream sourceStream)
        {
            string filename = GetFilename(id);

            _bucket.UploadFromStream(filename, sourceStream);
        }

        public void DownloadToStream(Guid id, Stream targetStream)
        {
            string filename = GetFilename(id);

            _bucket.DownloadToStreamByName(filename, targetStream);
        }

        public Stream GetDownloadStream(Guid id)
        {
            string filename = GetFilename(id);

            return _bucket.OpenDownloadStreamByName(filename);
        }

        public string GetFilename(Guid id)
        {
            return _filenameFactory(id);
        }
    }
}