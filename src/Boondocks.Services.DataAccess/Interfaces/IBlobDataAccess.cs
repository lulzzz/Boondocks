namespace Boondocks.Services.DataAccess.Interfaces
{
    using System;
    using System.IO;

    public interface IBlobDataAccess
    {
        void UploadFromStream(Guid id, Stream sourceStream);

        void DownloadToStream(Guid id, Stream targetStream);

        Stream GetDownloadStream(Guid id);

        string GetFilename(Guid id);
    }
}