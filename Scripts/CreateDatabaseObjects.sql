USE [Boondocks]
GO
/****** Object:  Table [dbo].[AgentVersions]    Script Date: 3/14/2018 2:17:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgentVersions](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceTypeId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[ImageId] [nvarchar](max) NOT NULL,
	[Logs] [nvarchar](max) NOT NULL,
	[IsDisabled] [bit] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_SupervisorVersions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationEnvironmentVariables]    Script Date: 3/14/2018 2:17:34 PM ******/
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
/****** Object:  Table [dbo].[ApplicationEvents]    Script Date: 3/14/2018 2:17:34 PM ******/
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
/****** Object:  Table [dbo].[ApplicationLogs]    Script Date: 3/14/2018 2:17:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceId] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[CreatedLocal] [datetime] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Applications]    Script Date: 3/14/2018 2:17:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Applications](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[DeviceTypeId] [uniqueidentifier] NOT NULL,
	[ApplicationVersionId] [uniqueidentifier] NULL,
	[AgentVersionId] [uniqueidentifier] NULL,
	[RootFileSystemVersionId] [uniqueidentifier] NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationVersions]    Script Date: 3/14/2018 2:17:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationVersions](
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ImageId] [nvarchar](100) NOT NULL,
	[Logs] [nvarchar](max) NULL,
	[IsDisabled] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_ApplicationVersions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceArchitectures]    Script Date: 3/14/2018 2:17:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceArchitectures](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_DeviceArchitectures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceEnvironmentVariables]    Script Date: 3/14/2018 2:17:34 PM ******/
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
/****** Object:  Table [dbo].[DeviceEvents]    Script Date: 3/14/2018 2:17:35 PM ******/
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
/****** Object:  Table [dbo].[Devices]    Script Date: 3/14/2018 2:17:35 PM ******/
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
	[AgentVersionId] [uniqueidentifier] NULL,
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
/****** Object:  Table [dbo].[DeviceStatus]    Script Date: 3/14/2018 2:17:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceStatus](
	[DeviceId] [uniqueidentifier] NOT NULL,
	[RootFileSystemVersion] [nvarchar](100) NULL,
	[ApplicationVersion] [nvarchar](100) NULL,
	[AgentVersion] [nvarchar](100) NULL,
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
/****** Object:  Table [dbo].[DeviceTypes]    Script Date: 3/14/2018 2:17:35 PM ******/
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
/****** Object:  Table [dbo].[RootFileSystemVersions]    Script Date: 3/14/2018 2:17:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RootFileSystemVersions](
	[Id] [uniqueidentifier] NOT NULL,
	[DeviceTypeId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[ImageId] [nvarchar](100) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_RootFileSystemVersions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AgentVersions]  WITH CHECK ADD  CONSTRAINT [FK_AgentVersions_DeviceTypes] FOREIGN KEY([DeviceTypeId])
REFERENCES [dbo].[DeviceTypes] ([Id])
GO
ALTER TABLE [dbo].[AgentVersions] CHECK CONSTRAINT [FK_AgentVersions_DeviceTypes]
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
ALTER TABLE [dbo].[ApplicationLogs]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationLogs_Devices] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Devices] ([Id])
GO
ALTER TABLE [dbo].[ApplicationLogs] CHECK CONSTRAINT [FK_ApplicationLogs_Devices]
GO
ALTER TABLE [dbo].[Applications]  WITH CHECK ADD  CONSTRAINT [FK_Applications_AgentVersions] FOREIGN KEY([AgentVersionId])
REFERENCES [dbo].[AgentVersions] ([Id])
GO
ALTER TABLE [dbo].[Applications] CHECK CONSTRAINT [FK_Applications_AgentVersions]
GO
ALTER TABLE [dbo].[Applications]  WITH CHECK ADD  CONSTRAINT [FK_Applications_ApplicationVersions] FOREIGN KEY([ApplicationVersionId])
REFERENCES [dbo].[ApplicationVersions] ([Id])
GO
ALTER TABLE [dbo].[Applications] CHECK CONSTRAINT [FK_Applications_ApplicationVersions]
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
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_AgentVersions] FOREIGN KEY([AgentVersionId])
REFERENCES [dbo].[AgentVersions] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_AgentVersions]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_Applications] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[Applications] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_Applications]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_ApplicationVersions] FOREIGN KEY([ApplicationVersionId])
REFERENCES [dbo].[ApplicationVersions] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_ApplicationVersions]
GO
ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_RootFileSystemVersions] FOREIGN KEY([RootFileSystemVersionId])
REFERENCES [dbo].[RootFileSystemVersions] ([Id])
GO
ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_RootFileSystemVersions]
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
