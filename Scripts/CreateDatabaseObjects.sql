USE [master]
GO
/****** Object:  Database [Boondocks]    Script Date: 3/26/2018 1:42:33 PM ******/
CREATE DATABASE [Boondocks]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Boondocks', FILENAME = N'E:\DevDB\Data\Boondocks.mdf' , SIZE = 81920KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Boondocks_log', FILENAME = N'E:\DevDB\Logs\Boondocks_log.ldf' , SIZE = 203008KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Boondocks] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Boondocks].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Boondocks] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Boondocks] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Boondocks] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Boondocks] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Boondocks] SET ARITHABORT OFF 
GO
ALTER DATABASE [Boondocks] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Boondocks] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Boondocks] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Boondocks] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Boondocks] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Boondocks] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Boondocks] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Boondocks] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Boondocks] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Boondocks] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Boondocks] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Boondocks] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Boondocks] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Boondocks] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Boondocks] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Boondocks] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Boondocks] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Boondocks] SET RECOVERY FULL 
GO
ALTER DATABASE [Boondocks] SET  MULTI_USER 
GO
ALTER DATABASE [Boondocks] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Boondocks] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Boondocks] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Boondocks] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Boondocks] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'Boondocks', N'ON'
GO
USE [Boondocks]
GO
/****** Object:  User [boondocks]    Script Date: 3/26/2018 1:42:33 PM ******/
CREATE USER [boondocks] FOR LOGIN [boondocks] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [boondocks]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [boondocks]
GO
/****** Object:  Table [dbo].[AgentVersions]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_SupervisorVersions] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationEnvironmentVariables]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_ApplicationEnvironmentVariables] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationEvents]    Script Date: 3/26/2018 1:42:33 PM ******/
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
/****** Object:  Table [dbo].[ApplicationLogs]    Script Date: 3/26/2018 1:42:33 PM ******/
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
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_ApplicationLogs] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Applications]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_Applications] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationVersions]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_ApplicationVersions] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceArchitectures]    Script Date: 3/26/2018 1:42:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceArchitectures](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_DeviceArchitectures] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceEnvironmentVariables]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_DeviceEnvironmentVariables] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceEvents]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_DeviceEvents] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Devices]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_Devices] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceStatus]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_DeviceStatus] PRIMARY KEY NONCLUSTERED 
(
	[DeviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeviceTypes]    Script Date: 3/26/2018 1:42:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeviceTypes](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CreatedUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_DeviceTypes] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RootFileSystemVersions]    Script Date: 3/26/2018 1:42:33 PM ******/
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
 CONSTRAINT [PK_RootFileSystemVersions] PRIMARY KEY NONCLUSTERED 
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
USE [master]
GO
ALTER DATABASE [Boondocks] SET  READ_WRITE 
GO
