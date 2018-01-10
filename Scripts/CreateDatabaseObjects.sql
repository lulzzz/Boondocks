/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.1000)
    Source Database Engine Edition : Microsoft SQL Server Express Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Standard Edition
    Target Database Engine Type : Standalone SQL Server
*/
USE [Boondocks]
GO
/****** Object:  User [boondocks]    Script Date: 1/10/2018 5:33:17 PM ******/
CREATE USER [boondocks] FOR LOGIN [boondocks] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [boondocks]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [boondocks]
GO
/****** Object:  Table [dbo].[ApplicationEnvironmentVariables]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationEnvironmentVariables](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_ApplicationEnvironmentVariables] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationEvents]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationEvents](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[EventType] [int] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_ApplicationEvents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Applications]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Applications](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[DeviceTypeId] [uniqueidentifier] NOT NULL,
	[ApplicationVersionId] [uniqueidentifier] NULL,
	[SupervisorVersionId] [uniqueidentifier] NULL,
	[RootFileSystemVersionId] [uniqueidentifier] NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationVersions]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationVersions](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IsDisabled] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_ApplicationVersions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceEnvironmentVariables]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceEnvironmentVariables](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_DeviceEnvironmentVariables] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceEvents]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceEvents](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [uniqueidentifier] NOT NULL,
	[EventType] [int] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_DeviceEvents_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Devices]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Devices](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[DeviceKey] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[ApplicationVersionId] [uniqueidentifier] NULL,
	[SupervisorVersionId] [uniqueidentifier] NULL,
	[RootFileSystemVersionId] [uniqueidentifier] NULL,
	[IsDisabled] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[ConfigurationVersion] [uniqueidentifier] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceStatus]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceStatus](
	[DeviceId] [uniqueidentifier] NOT NULL,
	[RootFileSystemVersionId] [uniqueidentifier] NULL,
	[ApplicationVersionId] [uniqueidentifier] NULL,
	[SupervisorVersionId] [uniqueidentifier] NULL,
	[LocalIpAddress] [nvarchar](max) NULL,
	[State] [int] NOT NULL,
	[Progress] [int] NULL,
	[UptimeSeconds] [int] NULL,
	[LastContactUtc] [datetime] NULL,
 CONSTRAINT [PK_DeviceStatus] PRIMARY KEY CLUSTERED 
(
	[DeviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceTypes]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceTypes](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_DeviceTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RootFileSystemVersions]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RootFileSystemVersions](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceTypeId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_RootFileSystemVersions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SupervisorVersions]    Script Date: 1/10/2018 5:33:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupervisorVersions](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceTypeId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_SupervisorVersions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ApplicationEnvironmentVariables]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationEnvironmentVariables_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO
ALTER TABLE [dbo].[ApplicationEnvironmentVariables] CHECK CONSTRAINT [FK_ApplicationEnvironmentVariables_Applications]
GO
ALTER TABLE [dbo].[ApplicationEvents]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationEvents_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO
ALTER TABLE [dbo].[ApplicationEvents] CHECK CONSTRAINT [FK_ApplicationEvents_Applications]
GO
ALTER TABLE [dbo].[Applications]  WITH CHECK ADD  CONSTRAINT [FK_Applications_DeviceTypes] FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceTypes] ([Id])
GO
ALTER TABLE [dbo].[Applications] CHECK CONSTRAINT [FK_Applications_DeviceTypes]
GO
ALTER TABLE [dbo].[Applications]  WITH CHECK ADD  CONSTRAINT [FK_Applications_RootFileSystemVersions] FOREIGN KEY([RootFileSystemVersionId])
REFERENCES [dbo].[RootFileSystemVersions] ([Id])
GO
ALTER TABLE [dbo].[Applications] CHECK CONSTRAINT [FK_Applications_RootFileSystemVersions]
GO
ALTER TABLE [dbo].[Applications]  WITH CHECK ADD  CONSTRAINT [FK_Applications_SupervisorVersions] FOREIGN KEY([SupervisorVersionId])
REFERENCES [dbo].[SupervisorVersions] ([Id])
GO
ALTER TABLE [dbo].[Applications] CHECK CONSTRAINT [FK_Applications_SupervisorVersions]
GO
ALTER TABLE [dbo].[Applications]  WITH CHECK ADD  CONSTRAINT [FK_Applications_SupervisorVersions1] FOREIGN KEY([SupervisorVersionId])
REFERENCES [dbo].[SupervisorVersions] ([Id])
GO
ALTER TABLE [dbo].[Applications] CHECK CONSTRAINT [FK_Applications_SupervisorVersions1]
GO
ALTER TABLE [dbo].[ApplicationVersions]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationVersions_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO
ALTER TABLE [dbo].[ApplicationVersions] CHECK CONSTRAINT [FK_ApplicationVersions_Applications]
GO
ALTER TABLE [dbo].[DeviceEnvironmentVariables]  WITH CHECK ADD  CONSTRAINT [FK_DeviceEnvironmentVariables_Devices] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([Id])
GO
ALTER TABLE [dbo].[DeviceEnvironmentVariables] CHECK CONSTRAINT [FK_DeviceEnvironmentVariables_Devices]
GO
ALTER TABLE [dbo].[DeviceEvents]  WITH CHECK ADD  CONSTRAINT [FK_DeviceEvents_Devices] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([Id])
GO
ALTER TABLE [dbo].[DeviceEvents] CHECK CONSTRAINT [FK_DeviceEvents_Devices]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_Applications]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_RootFileSystemVersions] FOREIGN KEY([RootFileSystemVersionId])
REFERENCES [dbo].[RootFileSystemVersions] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_RootFileSystemVersions]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_SupervisorVersions] FOREIGN KEY([SupervisorVersionId])
REFERENCES [dbo].[SupervisorVersions] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_SupervisorVersions]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_SupervisorVersions1] FOREIGN KEY([SupervisorVersionId])
REFERENCES [dbo].[SupervisorVersions] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_SupervisorVersions1]
GO
ALTER TABLE [dbo].[DeviceStatus]  WITH CHECK ADD  CONSTRAINT [FK_DeviceStatus_Devices] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([Id])
GO
ALTER TABLE [dbo].[DeviceStatus] CHECK CONSTRAINT [FK_DeviceStatus_Devices]
GO
ALTER TABLE [dbo].[RootFileSystemVersions]  WITH CHECK ADD  CONSTRAINT [FK_RootFileSystemVersions_DeviceTypes] FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceTypes] ([Id])
GO
ALTER TABLE [dbo].[RootFileSystemVersions] CHECK CONSTRAINT [FK_RootFileSystemVersions_DeviceTypes]
GO
ALTER TABLE [dbo].[SupervisorVersions]  WITH CHECK ADD  CONSTRAINT [FK_SupervisorVersions_DeviceTypes] FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceTypes] ([Id])
GO
ALTER TABLE [dbo].[SupervisorVersions] CHECK CONSTRAINT [FK_SupervisorVersions_DeviceTypes]
GO
ALTER TABLE [dbo].[SupervisorVersions]  WITH CHECK ADD  CONSTRAINT [FK_SupervisorVersions_DeviceTypes1] FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceTypes] ([Id])
GO
ALTER TABLE [dbo].[SupervisorVersions] CHECK CONSTRAINT [FK_SupervisorVersions_DeviceTypes1]
GO
