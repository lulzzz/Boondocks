namespace Boondocks.Services.DataAccess
{
    using System.Data;
    using System.Data.SqlClient;
    using Interfaces;

    //Temporary workaround. Need to implement a provider lookup:
    // https://weblog.west-wind.com/posts/2017/Nov/27/Working-around-the-lack-of-dynamic-DbProviderFactory-loading-in-NET-Core

    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlServerDbConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new System.ArgumentException("connectionString is null or whitespace.", nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_connectionString);
        }
    }
}