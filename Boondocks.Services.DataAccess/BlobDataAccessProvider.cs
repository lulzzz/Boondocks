using System;
using Boondocks.Services.DataAccess.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Boondocks.Services.DataAccess
{
    public class BlobDataAccessProvider : IBlobDataAccessProvider
    {
        public BlobDataAccessProvider(IBlobDataAccesConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            //Create the mongo client
            var mongoClient = new MongoClient(configuration.ConnectionString);

            //Get the database
            var database = mongoClient.GetDatabase(configuration.DatabaseName);

            //What the bucket?
            IGridFSBucket bucket = new GridFSBucket(database);

            //Create the individual accessors
            ApplicationVersionImages = new BlobDataAccess(bucket, id => $"ApplicationVersion_{id:N}.image");
            SupervisorVersionImages = new BlobDataAccess(bucket, id => $"SupervisorVersion_{id:N}.image");
        }

        public IBlobDataAccess ApplicationVersionImages { get; }

        public IBlobDataAccess SupervisorVersionImages { get; }
    }
}