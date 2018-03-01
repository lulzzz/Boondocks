# Boondocks Agent
This is the code that runs on the end device to manage updates.

## Concepts / Terms
- **Agent** refers to the container that manages the device and downloads / installs applications / agents. The reserved name for the agent container is `boondocks-agent`.
- **Application** This is the end user application that is running on the device. The reserved name for the application container is `boondocks-application`.

## Components
- **AgentHost** This serves as the entry point and main coordination point for all monitoring and update operations.

## Update Services
These services update the agent and application.

- **AgentUpdateService** Responsible for downloading new agent images and starting them up as containers.
- **ApplicationUpdateService** Responsible for downloading new application images and starting them up as containers.

## Heartbeat
A "heartbeat" is periodically sent in order to:
- Let the server know that the defvice is alive.
- Let the server know what version of agent / application is running on the device.
- We use the concept of a `ConfigurationId` to minimize communication needs between the agent and the device api. The current `ConfigurationId` (a Guid) is returned during every heartbeat operation. If this `ConfigurationId` changes, the agent will then make an additional call to get the current configuration for the device.

## Agent Update Process 
- The new agent image is downloaded.
- The currently running agent container is renamed from `boondocks-agent` to `boondocks-agent-outgoing`.
- The new agent container is created as `boondocks-agent` and started.
- The old container `boondocks-agent-outgoing` is forcibly removed.
- Images are pruned.

## Application Update Process
- The new application image is downloaded.
- The old application container is removed.
- A new application container is created as `boondocks-application`.
- The new application container is started.
- Images are pruned.

## Notes
- If new version information is available for both the agent and the applciation at the same time, the agent will be updated first.