# Boondocks Relational Data
The ultimate goal is to support multiple relational databases (e.g. MySql, PostgreSQL, etc), the initial implementation is being done on SQL Server.

## EntityBase
Base type for many of the entities described here.
A device type (e.g. Raspberry Pi 2, Raspberry Pi 3)

| Column | Type | Notes |
| ------ | ---- | ----- |
|Id|Guid|
|CreatedUtc|DateTime|

## DeviceType : EntityBase
A device type (e.g. Raspberry Pi 2, Raspberry Pi 3)

| Column | Type | Notes |
| ------ | ---- | ----- |
|Name|string| |

## Devices : EntityBase
Represents a physical device / module.

| Column | Type | Notes |
| ------ | ---- | ----- |
|Name|string|Friendly name|
|Device Key|Guid|Used to sign JWTs in order to identify this device.|
|ApplicationId|Guid|The application that this device belongs to|
|AppliationVersionId|Guid?|If specified, overrides the value from the application.|
|AgentVersionId|Guid?|If specified, overrides the value from the application.|
|RootFileSystemId|Guid?||
|IsDisabled|bool||
|IsDeleted|bool||

## DeviceStatus
ConfgurationVersion - this should be updated whenever the configuration of the device changes (that includes root file system, application version, environment variables).

| Column | Type | Notes |
| ------ | ---- | ----- |
|DeviceId            |Guid||
|RootFileSystem      |string||
|ApplicationVersion  |string||
|AgentVersion        |string||
|LocalIpAddress      |string||
|State               |int||
|Progress            |int||
|ConfigurationVersion|Guid|Updated whenever the configuration of the device changes.|

## DeviceEnvironmentVariables : EntityBase
These values will be passed to application container running on the device. If the same variable name is used for an application level environment variable, this value takes precedence.

| Column | Type | Notes |
| ------ | ---- | ----- |
|DeviceId|Guid||
|Name|string|The name of the variable.|
|Value|string|The value of the variable.|

## ApplicationEnvironmentVariables : EntityBase
These variables will be presented to all of the devices in the specified application. Values can be overriden at the device level.

| Column | Type | Notes |
| ------ | ---- | ----- |
|ApplicationId|Guid||
|Name|string|The name of the variable.|
|Value|string|The value of the variable.|

## DeviceEvents : EntityBase
Events for a given device.

| Column | Type | Notes |
| ------ | ---- | ----- |
|DeviceId|Guid||
|EventType|int||
|Message|string||

## Applications : EntityBase
Devices are grouped into applications.

| Column | Type | Notes |
| ------ | ---- | ----- |
|Name|string||
|DeviceTypeId||
|ApplicationVersionId||
|SupervisorVersionId||

## ApplicationEvents : EntityBase
Application specific events.

| Column | Type | Notes |
| ------ | ---- | ----- |
|EventType|int||
|Message|string||

## ApplicationLogs : EntityBase

| Column | Type | Notes |
| ------ | ---- | ----- |
|DeviceId|Guid||
|Type|int||
|Message|string|
|CreatedLocal|The local time at which the log event was created.|


## AgentVersions : EntityBase
Agent versions.

| Column | Type | Notes |
| ------ | ---- | ----- |
|DeviceTypeId|Guid||
|Name|string||
