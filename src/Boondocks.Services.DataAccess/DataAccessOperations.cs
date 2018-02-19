namespace Boondocks.Services.DataAccess
{
    using System;
    using System.Data;
    using System.Linq;
    using Contracts;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using Domain;

    public static class DataAccessOperations
    {
        public static DeviceEvent InsertDeviceEvent(
            this IDbConnection connection,
            IDbTransaction transaction,
            Guid deviceId,
            DeviceEventType eventType,
            string message)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var deviceEvent = new DeviceEvent
            {
                DeviceId = deviceId,
                EventType = eventType,
                Message = message
            }.SetNew();

            connection.Insert(deviceEvent, transaction);

            return deviceEvent;
        }

        public static ApplicationEvent InsertApplicationEvent(
            this IDbConnection connection,
            IDbTransaction transaction,
            Guid applicationId,
            ApplicationEventType eventType,
            string message)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var applicationEvent = new ApplicationEvent
            {
                ApplicationId = applicationId,
                EventType = eventType,
                Message = message
            }.SetNew();

            connection.Insert(applicationEvent, transaction);

            return applicationEvent;
        }

        /// <summary>
        ///     Inserts a device (and corresponding DeviceStatus) into the database.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="name"></param>
        /// <param name="applicationId"></param>
        /// <param name="deviceKey"></param>
        /// <param name="transaction">A transaction is required.</param>
        /// <returns></returns>
        public static Device InsertDevice(
            this IDbConnection connection,
            IDbTransaction transaction,
            string name,
            Guid applicationId,
            Guid? deviceKey)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var device = new Device
            {
                Name = name,
                ApplicationId = applicationId,
                DeviceKey = deviceKey ?? Guid.NewGuid(),
                ConfigurationVersion = Guid.NewGuid()
            }.SetNew();

            var deviceStatus = new DeviceStatus
            {
                DeviceId = device.Id,
                State = DeviceState.New
            };

            //Insert these 
            connection.Insert(device, transaction);
            connection.Insert(deviceStatus, transaction);

            return device;
        }

        /// <summary>
        ///     Setting the Id and CreatedUtc properties for a new entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T SetNew<T>(this T entity) where T : EntityBase
        {
            //Allow callers to specify their own id
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();

            //Always set a fresh created date.
            entity.CreatedUtc = DateTime.UtcNow;

            //Return the same entity because fluent api's rock.
            return entity;
        }

        /// <summary>
        ///     Gets a device key given a device id.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="deviceId"></param>
        /// <returns>The DeviceKey if found, null otherwise.</returns>
        public static Guid? GetDeviceKey(this IDbConnection connection, Guid deviceId)
        {
            const string sql = "select DeviceKey from Devices where Id = @DeviceId";

            return connection
                .QueryFirstOrDefault<DeviceKeyFromDatabase>(sql, new {DeviceId = deviceId})?.DeviceKey;
        }

        public static DeviceEnvironmentVariable InsertDeviceEnvironmentVariable(
            this IDbConnection connection,
            IDbTransaction transaction,
            Guid deviceId,
            string name,
            string value)
        {
            var variable = new DeviceEnvironmentVariable
            {
                DeviceId = deviceId,
                Name = name,
                Value = value
            }.SetNew();

            connection.Insert(variable, transaction);

            return variable;
        }

        public static ApplicationEnvironmentVariable InsertApplicationEnvironmentVariable(
            this IDbConnection connection,
            IDbTransaction transaction,
            Guid applicationID,
            string name,
            string value)
        {
            var variable = new ApplicationEnvironmentVariable
            {
                ApplicationId = applicationID,
                Name = name,
                Value = value
            }.SetNew();

            connection.Insert(variable, transaction);

            return variable;
        }

        /// <summary>
        ///     Sets a new device configuration for a single device. This is done to notify the the device that it needs to
        ///     download a new configuration.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="deviceId"></param>
        public static void SetNewDeviceConfigurationVersionForDevice(
            this IDbConnection connection,
            IDbTransaction transaction,
            Guid deviceId)
        {
            const string sql = "update devices set " +
                               "  ConfigurationVersion = @ConfigurationVersion " +
                               "where " +
                               "  Id = @DeviceId" +
                               "  and IsDisabled = 0" +
                               "  and IsDeleted = 0 ";

            var parameters = new
            {
                ConfigurationVersion = Guid.NewGuid(),
                DeviceId = deviceId
            };

            connection.Execute(sql, parameters, transaction);
        }

        /// <summary>
        ///     Sets a new device configuration all of the devices in a given application. This is done to notify the the device
        ///     that it needs to
        ///     download a new configuration.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="applicationId"></param>
        public static void SetNewDeviceConfigurationVersionForApplication(
            this IDbConnection connection,
            IDbTransaction transaction,
            Guid applicationId)
        {
            const string sql = "update devices set " +
                               "  ConfigurationVersion = @ConfigurationVersion " +
                               "where " +
                               "  ApplicationId = @ApplicationId " +
                               "  and IsDisabled = 0" +
                               "  and IsDeleted = 0 ";

            var parameters = new
            {
                ConfigurationVersion = Guid.NewGuid(),
                ApplicationID = applicationId
            };

            connection.Execute(sql, parameters, transaction);
        }

        public static DeviceEnvironmentVariable[] GetDeviceEnvironmentVariables(this IDbConnection connection,
            Guid deviceId)
        {
            const string sql = "select * from DeviceEnvironmentVariables where DeviceId = @deviceId";

            return connection
                .Query<DeviceEnvironmentVariable>(sql, new {deviceId})
                .ToArray();
        }

        public static ApplicationEnvironmentVariable[] GetApplicationEnvironmentVariables(this IDbConnection connection,
            Guid applicationId)
        {
            const string sql = "select * from ApplicationEnvironmentVariables where ApplicationId = @applicationId";

            return connection
                .Query<ApplicationEnvironmentVariable>(sql, new {applicationId})
                .ToArray();
        }


        public static bool IsApplicationVersionNameInUse(this IDbConnection connection, Guid applicationId, string name)
        {
            //https://stackoverflow.com/a/39023427/232566

            const string sql =
                "select count(1) from ApplicationVersions where ApplicationId = @applicationId and Name = @name";

            return connection.ExecuteScalar<bool>(sql, new {applicationId, name});
        }

        public static bool IsAgentVersionNameInUse(this IDbConnection connection, Guid deviceArhitectureId, string name)
        {
            //https://stackoverflow.com/a/39023427/232566

            const string sql =
                "select count(1) from AgentVersions where DeviceArchitectureId = @deviceArhitectureId and Name = @name";

            return connection.ExecuteScalar<bool>(sql, new { deviceArhitectureId, name });
        }

        public static bool IsApplicationVersionImageIdInUse(this IDbConnection connection, Guid applicationId,
            string imageId)
        {
            //https://stackoverflow.com/a/39023427/232566

            const string sql =
                "select count(1) from ApplicationVersions where ApplicationId = @applicationId and ImageId = @imageId";

            return connection.ExecuteScalar<bool>(sql, new {applicationId, imageId});
        }

        private class DeviceKeyFromDatabase
        {
            public Guid DeviceKey { get; set; }
        }
    }
}