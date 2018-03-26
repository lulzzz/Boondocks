using System;
using System.Data;

namespace Boondocks.Base.Data
{
    /// <summary>
    /// Represents a database connection created for the lifetime of the
    /// request.  The consumer should inject and place the instance within
    /// an using statement within a root component responsible for handeling
    /// the request. (i.e. command-handler).
    /// </summary>
    public interface IRepositoryContext<TDatabase> : IDisposable
        where TDatabase : DbSettings
    {
        /// <summary>
        /// Returns an opened connection.
        /// </summary>
        /// <returns>Reference to provider specific database connection.</returns>
        IDbConnection OpenConn();
    }
}