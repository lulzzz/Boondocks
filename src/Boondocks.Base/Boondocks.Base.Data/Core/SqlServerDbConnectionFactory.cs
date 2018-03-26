using System.Data;
using System.Data.SqlClient;

namespace Boondocks.Base.Data.Core
{
    //Temporary workaround. Need to implement a provider lookup:
    // https://weblog.west-wind.com/posts/2017/Nov/27/Working-around-the-lack-of-dynamic-DbProviderFactory-loading-in-NET-Core

    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection Create(string connectionString) 
            => new SqlConnection(connectionString);
    }
}