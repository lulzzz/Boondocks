using System;
using System.Data;
using System.Linq;
using Boondocks.Services.Contracts;
using Dapper;

namespace Boondocks.Services.DataAccess
{
    public static class DataAccessOperations
    {
        public static DeviceType GetDeviceType(this IDbConnection connection, Guid id)
        {
            return connection
                .QuerySingleOrDefault<DeviceType>(
                    "select Id, Name, CreatedUtc from DeviceTypes where Id = @id",
                    new { id });
        }

        public static Application GetApplication(this IDbConnection connection, Guid id)
        {
            return connection
                .QuerySingleOrDefault("select * from Applications where Id = @id", 
                new { id });
        }

        public static Device GetDevice(this IDbConnection connection, Guid id)
        {
            return connection
                .QuerySingleOrDefault("select * from Device where Id = @id", new { id });
        }

        /// <summary>
        /// Setting the Id and CreatedUtc properties for a new entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T SetNew<T>(this T entity) where T : EntityBase
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedUtc = DateTime.UtcNow;

            return entity;
        }
    }
}