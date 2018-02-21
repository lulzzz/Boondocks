# Boondocks
.NET Core stack to support delivery of applications in far flung IoT kind of places.

## Overview
Boondocks is used for deploying and updating embedded Linux applications in IoT scenarios.

## Main Components
- **Device** This is the end device running Linux.
   - **Agent** This is the container that runs on the end device
   - **Application** This is the user application served up by the platform
- **Services**
   - **DeviceApi** The agent talks to this RESTful API to report status and download configuration information.
   - **ManagementApi** This is the RESTful API for managing a Boondocks installation. This service handles things such as device provisioning, application management, etc
- **Command Line Interface (CLI)** This tool is useful for provisioning devices, managing applications and building supervisor / application images.
- **Docker Registry** Boondocks uses a private Docker registry to service up agent / application images.

## Setting Up
This is a work in progress.

## Third Party
Boondocks uses these great open source projects:
- [resin.io](http://resin.io)
- [.NET Core](https://github.com/dotnet/core)
- [Dapper.NET](https://github.com/StackExchange/Dapper)
- [Autofac](https://autofac.org/)
- [Serilog](https://serilog.net/)
- [Docker.DotNet](https://github.com/Microsoft/Docker.DotNet)
- [Swagger](https://swagger.io/)
- [Json.NET](https://www.newtonsoft.com/json)