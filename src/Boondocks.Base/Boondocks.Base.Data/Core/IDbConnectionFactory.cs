using System;
using System.Data;

namespace Boondocks.Base.Data.Core
{
    /// <summary>
    /// Interface implemented by a component responsible for creating the 
    /// correct derived database connection for the database being used.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Returns connection implementation for a specific database.
        /// </summary>
        /// <param name="connectionString">The connection string to use 
        /// to create the connection</param>
        /// <returns>Non opened database connection instance.</returns>
        IDbConnection Create(string connectionString);
    }
}