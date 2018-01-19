using Boondocks.Services.DataAccess.Interfaces;

namespace Boondocks.Services.DataAccess
{
    public class BlobDataAccessConfiguration : IBlobDataAccesConfiguration
    {
        public BlobDataAccessConfiguration(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }

        public string ConnectionString { get; }

        public string DatabaseName { get; }
    }
}