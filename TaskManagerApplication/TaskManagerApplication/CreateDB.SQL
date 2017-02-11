
/*Create Database*/
		USE [master]
	GO

	CREATE DATABASE [TaskManagerDB]
	 CONTAINMENT = NONE
	 ON  PRIMARY 
	( NAME = N'TaskManagerDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\TaskManagerDB.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
	 LOG ON 
	( NAME = N'TaskManagerDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\TaskManagerDB_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
	GO

	ALTER DATABASE [TaskManagerDB] SET COMPATIBILITY_LEVEL = 120
	GO

	IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
	begin
	EXEC [TaskManagerDB].[dbo].[sp_fulltext_database] @action = 'enable'
	end
	GO

	ALTER DATABASE [TaskManagerDB] SET ANSI_NULL_DEFAULT OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET ANSI_NULLS OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET ANSI_PADDING OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET ANSI_WARNINGS OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET ARITHABORT OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET AUTO_CLOSE OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET AUTO_SHRINK OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET AUTO_UPDATE_STATISTICS ON 
	GO

	ALTER DATABASE [TaskManagerDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET CURSOR_DEFAULT  GLOBAL 
	GO

	ALTER DATABASE [TaskManagerDB] SET CONCAT_NULL_YIELDS_NULL OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET NUMERIC_ROUNDABORT OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET QUOTED_IDENTIFIER OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET RECURSIVE_TRIGGERS OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET  DISABLE_BROKER 
	GO

	ALTER DATABASE [TaskManagerDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET TRUSTWORTHY OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET PARAMETERIZATION SIMPLE 
	GO

	ALTER DATABASE [TaskManagerDB] SET READ_COMMITTED_SNAPSHOT OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET HONOR_BROKER_PRIORITY OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET RECOVERY SIMPLE 
	GO

	ALTER DATABASE [TaskManagerDB] SET  MULTI_USER 
	GO

	ALTER DATABASE [TaskManagerDB] SET PAGE_VERIFY CHECKSUM  
	GO

	ALTER DATABASE [TaskManagerDB] SET DB_CHAINING OFF 
	GO

	ALTER DATABASE [TaskManagerDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
	GO

	ALTER DATABASE [TaskManagerDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
	GO

	ALTER DATABASE [TaskManagerDB] SET DELAYED_DURABILITY = DISABLED 
	GO

	ALTER DATABASE [TaskManagerDB] SET  READ_WRITE 
	GO



/*End Create Database*/

use [TaskManagerDB]
go

/*Create Categories table*/
	IF object_id('Categories', 'U') IS NULL
		CREATE TABLE [TaskManagerDB].[dbo].[Categories]
		(
			[ID] [int] Identity(1,1) PRIMARY KEY,
			[Name] [nchar](10) NOT NULL,
		) 
	GO
/* Create Properties table*/
	IF object_id('Priorities', 'U') IS NULL
	CREATE TABLE [TaskManagerDB].[dbo].[Priorities]
		(
			[ID] [int] Identity(1,1) PRIMARY KEY,
			[Setting] [nchar](10) NOT NULL,
		)
	GO

/*Create Tasks table*/
	IF object_id('Tasks', 'U') IS NULL
		CREATE TABLE [TaskManagerDB].[dbo].[Tasks](
			[ID] [int] Identity(1,1) PRIMARY KEY,
			[CategoryID] [int] NOT NULL,
			[PriorityID] [int] NULL,
			[Name] [nchar](10) NOT NULL,
			[Description] [nvarchar](50) NULL,
			[Deadline] [datetime] NULL
	GO

	ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Categories] FOREIGN KEY([CategoryID])
	REFERENCES [TaskManagerDB].[dbo].[Categories] ([ID])
	GO

	ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_Categories]
	GO

	ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Priorities] FOREIGN KEY([PriorityID])
	REFERENCES [dbo].[Priorities] ([ID])
	GO
/*End Create Tasks table*/


insert into TaskManagerDB.dbo.Priorities
values (N'High')
insert into TaskManagerDB.dbo.Priorities
values (N'Medium')
insert into TaskManagerDB.dbo.Priorities
values (N'Low')


IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Tasks]') 
         AND name = 'IsCompleted'
)
ALTER TABLE dbo.Tasks ADD IsCompleted BIT default 'False'

update dbo.Tasks
set IsCompleted = 'False'

