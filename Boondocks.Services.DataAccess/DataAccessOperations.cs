﻿using System;
using System.Data;
using System.Linq;
using System.Text;
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

            const string sql = "insert DeviceEvents " +
                               "(" +
                               "  Id, " +
                               "  DeviceId, " +
                               "  EventType, " +
                               "  Message, " +
                               "  CreatedUtc" +
                               ")";

            connection.Execute(sql, deviceEvent, transaction);

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

            const string sql = "insert DeviceEvents " +
                               "(" +
                               "  Id, " +
                               "  DeviceId, " +
                               "  EventType, " +
                               "  Message, " +
                               "  CreatedUtc" +
                               ")";

            connection.Execute(sql, applicationEvent, transaction);

            return applicationEvent;
        }

        /// <summary>
        /// Inserts a device (and corresponding DeviceStatus) into the database.
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

            var device = new Device()
            {
                Name = name,
                ApplicationId = applicationId,
                DeviceKey = deviceKey ?? Guid.NewGuid(),
                ConfigurationVersion = Guid.NewGuid()
            }.SetNew();

            var deviceStatus = new DeviceStatus()
            {
                DeviceId = device.Id,
                State = DeviceState.New
            };

            const string deviceSql = "insert Devices " +
                               "(" +
                               "  Id, " +
                               "  Name, " +
                               "  ApplicationId, " +
                               "  DeviceKey, " +
                               "  CreatedUtc, " +
                               "  IsDisabled, " +
                               "  IsDeleted," +
                               "  ConfigurationVersion" +
                               ") values (" +
                               "  @Id, " +
                               "  @Name, " +
                               "  @ApplicationId, " +
                               "  @DeviceKey, " +
                               "  @CreatedUtc, " +
                               "  0, " +
                               "  0, " +
                               "  @ConfigurationVersion" +
                               ")";

            connection.Execute(deviceSql, device, transaction);

            const string deviceStatusSql = "insert DeviceStatus " +
                                           "( " +
                                           "  DeviceId, " +
                                           "  State " + 
                                           ") values (" +
                                           "  @DeviceId, " +
                                           "  @State" +
                                           ")";

            connection.Execute(deviceStatusSql, deviceStatus, transaction);

            return device;
        }

        /// <summary>
        /// Setting the Id and CreatedUtc properties for a new entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T SetNew<T>(this T entity) where T : EntityBase
        {
            //Allow callers to specify their own id
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            //Always set a fresh created date.
            entity.CreatedUtc = DateTime.UtcNow;

            //Return the same entity because fluent api's rock.
            return entity;
        }

        /// <summary>
        /// Gets a device key given a device id.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="deviceId"></param>
        /// <returns>The DeviceKey if found, null otherwise.</returns>
        public static Guid? GetDeviceKey(this IDbConnection connection, Guid deviceId)
        {
            const string sql = "select DeviceKey from Devices where Id = @DeviceId";

            return connection
                .QueryFirstOrDefault<DeviceKeyFromDatabase>(sql, new { DeviceId = deviceId })?.DeviceKey;
        }

        private class DeviceKeyFromDatabase
        {
            public Guid DeviceKey { get; set; }
        }

        public static DeviceEnvironmentVariable GetDeviceEnvironmentVariable(this IDbConnection connection, Guid id)
        {
            return connection
                .QuerySingleOrDefault<DeviceEnvironmentVariable>("select * from DeviceEnvironmentVariables where Id = @id", new { id });
        }

        public static DeviceEnvironmentVariable InsertDeviceEnvironmentVariable(
            this IDbConnection connection, 
            IDbTransaction transaction, 
            Guid deviceId, 
            string name, 
            string value)
        {
            DeviceEnvironmentVariable variable = new DeviceEnvironmentVariable()
            {
                DeviceId = deviceId,
                Name = name,
                Value = value
            }.SetNew();

            const string sql = "insert DeviceEvents " +
                               "(" +
                               "  Id, " +
                               "  DeviceId, " +
                               "  Name, " +
                               "  Value, " +
                               "  CreatedUtc" +
                               ") values ( " +
                               "  @Id, " +
                               "  @DeviceId," +
                               "  @Name, " +
                               "  @Value, " +
                               "  @CreatedUtc" +
                               ")";

            connection.Execute(sql, variable, transaction);

            return variable;
        }

        /// <summary>
        /// Sets a new device configuration for a single device. This is done to notify the the device that it needs to 
        /// download a new configuration.
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
                               "  ConfigurationVersion = @ConfigurationVersion" +
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
        /// Sets a new device configuration all of the devices in a given application. This is done to notify the the device that it needs to 
        /// download a new configuration.
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
                               "  ConfigurationVersion = @ConfigurationVersion" +
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


    }
}