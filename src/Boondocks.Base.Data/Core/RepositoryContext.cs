using System;
using System.Data;
using NetFusion.Common.Extensions;

/// <summary>
/// Tracks an open connection to a database for the lifetime of the request.
/// This class is responsible for creating and opening the database on first use.
/// An instance of this class is best used within a using statement to dispose
/// the connection from a root object handling request (i.e. command-handler).
/// If not disposed within a using statement, the DI container will dispose it
/// at the end of the request since the instance is registers per-lifetime scope.
/// </summary>
namespace Boondocks.Base.Data.Core
{
    public class RepositoryContext<TDatabase> : IRepositoryContext<TDatabase>
        where TDatabase : DbSettings
    {
        private readonly Lazy<IDbConnection> _connection;

        public RepositoryContext(TDatabase dbSettings, IDbConnectionFactory connFactory)
        {
            if (dbSettings == null) throw new ArgumentNullException(nameof(dbSettings));
            if (connFactory == null) throw new ArgumentNullException(nameof(connFactory));

            Console.WriteLine(dbSettings.ToJson());

            _connection = new Lazy<IDbConnection>(() => connFactory.Create(dbSettings.ConnectionString));
        }

        public IDbConnection OpenConn()
        {
            // If connection has already been requested, return the created instance.
            if (_connection.IsValueCreated)
            {
                return _connection.Value;
            }
            
            // Created and open the connection.
            _connection.Value?.Open();
            return _connection.Value;
        }

        public void Dispose()
        {
            if (_connection.IsValueCreated)
            {
                _connection.Value?.Dispose();
            }
        }
    }
}

