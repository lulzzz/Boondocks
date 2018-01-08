/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2017 (14.0.1000)
    Source Database Engine Edition : Microsoft SQL Server Express Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Express Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [master]
GO

/****** Object:  Database [Boondocks]    Script Date: 1/5/2018 2:11:53 PM ******/
CREATE DATABASE [Boondocks]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Boondocks', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Boondocks.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Boondocks_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Boondocks_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
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

ALTER DATABASE [Boondocks] SET  READ_WRITE 
GO

