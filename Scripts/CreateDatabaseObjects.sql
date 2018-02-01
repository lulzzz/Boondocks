/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.1000)
    Source Database Engine Edition : Microsoft SQL Server Express Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Standard Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [master]
GO
/****** Object:  Database [Boondocks]    Script Date: 1/31/2018 4:59:18 PM ******/
CREATE DATABASE [Boondocks]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Boondocks', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Boondocks_dev.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Boondocks_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Boondocks_dev_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Boondocks] SET COMPATIBILITY_LEVEL = 140
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
ALTER DATABASE [Boondocks] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Boondocks] SET  MULTI_USER 
GO
ALTER DATABASE [Boondocks] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Boondocks] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Boondocks] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Boondocks] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Boondocks] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Boondocks] SET QUERY_STORE = OFF
GO
USE [Boondocks]
GO
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [Boondocks]
GO
/****** Object:  User [boondocks]    Script Date: 1/31/2018 4:59:18 PM ******/
CREATE USER [boondocks] FOR LOGIN [boondocks] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [boondocks]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [boondocks]
GO
/****** Object:  Table [dbo].[ApplicationEnvironmentVariables]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[ApplicationEvents]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[Applications]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[ApplicationVersions]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[DeviceEnvironmentVariables]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[DeviceEvents]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[Devices]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[DeviceStatus]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[DeviceTypes]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[RootFileSystemVersions]    Script Date: 1/31/2018 4:59:18 PM ******/
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
/****** Object:  Table [dbo].[SupervisorVersions]    Script Date: 1/31/2018 4:59:18 PM ******/
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
SET ANSI_PADDING ON
GO
/****** Object:  Index [CI_ApplicationIdImageIdUnique]    Script Date: 1/31/2018 4:59:18 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [CI_ApplicationIdImageIdUnique] ON [dbo].[ApplicationVersions]
(
	[ApplicationId] ASC,
	[ImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [CI_ApplicationIdNameUnique]    Script Date: 1/31/2018 4:59:18 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [CI_ApplicationIdNameUnique] ON [dbo].[ApplicationVersions]
(
	[ApplicationId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
USE [master]
GO
ALTER DATABASE [Boondocks] SET  READ_WRITE 
GO
