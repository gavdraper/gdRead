USE [master]
GO
/****** Object:  Database [gdRead]    Script Date: 18/01/2014 08:04:02 ******/
CREATE DATABASE [gdRead]
GO
ALTER DATABASE [gdRead] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [gdRead].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [gdRead] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [gdRead] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [gdRead] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [gdRead] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [gdRead] SET ARITHABORT OFF 
GO
ALTER DATABASE [gdRead] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [gdRead] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [gdRead] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [gdRead] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [gdRead] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [gdRead] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [gdRead] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [gdRead] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [gdRead] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [gdRead] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [gdRead] SET  DISABLE_BROKER 
GO
ALTER DATABASE [gdRead] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [gdRead] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [gdRead] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [gdRead] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [gdRead] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [gdRead] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [gdRead] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [gdRead] SET RECOVERY FULL 
GO
ALTER DATABASE [gdRead] SET  MULTI_USER 
GO
ALTER DATABASE [gdRead] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [gdRead] SET DB_CHAINING OFF 
GO
USE [gdRead]
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 18/01/2014 08:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
@Application NVARCHAR(60),
@PageIndex INT = 0,
@PageSize INT = 15,
@TotalCount INT OUTPUT
)
AS 

SET NOCOUNT ON

DECLARE @FirstTimeUTC DATETIME
DECLARE @FirstSequence INT
DECLARE @StartRow INT
DECLARE @StartRowIndex INT

SELECT 
    @TotalCount = COUNT(1) 
FROM 
    [ELMAH_Error]
WHERE 
    [Application] = @Application

-- Get the ID of the first error for the requested page

SET @StartRowIndex = @PageIndex * @PageSize + 1

IF @StartRowIndex <= @TotalCount
BEGIN

    SET ROWCOUNT @StartRowIndex

    SELECT  
        @FirstTimeUTC = [TimeUtc],
        @FirstSequence = [Sequence]
    FROM 
        [ELMAH_Error]
    WHERE   
        [Application] = @Application
    ORDER BY 
        [TimeUtc] DESC, 
        [Sequence] DESC

END
ELSE
BEGIN

    SET @PageSize = 0

END

-- Now set the row count to the requested page size and get
-- all records below it for the pertaining application.

SET ROWCOUNT @PageSize

SELECT 
    errorId     = [ErrorId], 
    application = [Application],
    host        = [Host], 
    type        = [Type],
    source      = [Source],
    message     = [Message],
    [user]      = [User],
    statusCode  = [StatusCode], 
    time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
FROM 
    [ELMAH_Error] error
WHERE
    [Application] = @Application
AND
    [TimeUtc] <= @FirstTimeUTC
AND 
    [Sequence] <= @FirstSequence
ORDER BY
    [TimeUtc] DESC, 
    [Sequence] DESC
FOR
    XML AUTO


GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 18/01/2014 08:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
@Application NVARCHAR(60),
@ErrorId UNIQUEIDENTIFIER
)
AS

SET NOCOUNT ON

SELECT 
    [AllXml]
FROM 
    [ELMAH_Error]
WHERE
    [ErrorId] = @ErrorId
AND
    [Application] = @Application


GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 18/01/2014 08:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
@ErrorId UNIQUEIDENTIFIER,
@Application NVARCHAR(60),
@Host NVARCHAR(30),
@Type NVARCHAR(100),
@Source NVARCHAR(60),
@Message NVARCHAR(500),
@User NVARCHAR(50),
@AllXml NVARCHAR(MAX),
@StatusCode INT,
@TimeUtc DATETIME
)
AS

SET NOCOUNT ON

INSERT
INTO
    [ELMAH_Error]
    (
        [ErrorId],
        [Application],
        [Host],
        [Type],
        [Source],
        [Message],
        [User],
        [AllXml],
        [StatusCode],
        [TimeUtc]
    )
VALUES
    (
        @ErrorId,
        @Application,
        @Host,
        @Type,
        @Source,
        @Message,
        @User,
        @AllXml,
        @StatusCode,
        @TimeUtc
    )


GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 18/01/2014 08:04:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[User_Id] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[UserId] [nvarchar](128) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY CLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Feed]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feed](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NULL,
	[Url] [nvarchar](300) NULL,
	[LastChecked] [datetime] NULL,
 CONSTRAINT [PK_dbo.Feeds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Post]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Post](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Url] [nvarchar](300) NULL,
	[Summary] [nvarchar](max) NULL,
	[Content] [nvarchar](max) NULL,
	[PublishDate] [datetime] NOT NULL,
	[DateFetched] [datetime] NOT NULL,
	[FeedId] [int] NULL,
 CONSTRAINT [PK_dbo.Posts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Subscription]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscription](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FeedId] [int] NULL,
 CONSTRAINT [PK_dbo.Subscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[SubscriptionPostRead]    Script Date: 18/01/2014 08:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionPostRead](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionId] [int] NOT NULL,
	[PostId] [int] NOT NULL,
 CONSTRAINT [PK_SubscriptionPostRead] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_User_Id]    Script Date: 18/01/2014 08:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_User_Id] ON [dbo].[AspNetUserClaims]
(
	[User_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 18/01/2014 08:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_RoleId]    Script Date: 18/01/2014 08:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 18/01/2014 08:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ELMAH_Error_App_Time_Seq]    Script Date: 18/01/2014 08:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_ELMAH_Error_App_Time_Seq] ON [dbo].[ELMAH_Error]
(
	[Application] ASC,
	[TimeUtc] DESC,
	[Sequence] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_Feed_Id]    Script Date: 18/01/2014 08:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_Feed_Id] ON [dbo].[Post]
(
	[FeedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
/****** Object:  Index [IX_Feed_Id]    Script Date: 18/01/2014 08:04:03 ******/
CREATE NONCLUSTERED INDEX [IX_Feed_Id] ON [dbo].[Subscription]
(
	[FeedId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO
ALTER TABLE [dbo].[Feed] ADD  DEFAULT (getdate()) FOR [LastChecked]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_User_Id] FOREIGN KEY([User_Id])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_User_Id]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[Post]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Posts_dbo.Feeds_Feed_Id] FOREIGN KEY([FeedId])
REFERENCES [dbo].[Feed] ([Id])
GO
ALTER TABLE [dbo].[Post] CHECK CONSTRAINT [FK_dbo.Posts_dbo.Feeds_Feed_Id]
GO
ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Subscriptions_dbo.Feeds_Feed_Id] FOREIGN KEY([FeedId])
REFERENCES [dbo].[Feed] ([Id])
GO
ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_dbo.Subscriptions_dbo.Feeds_Feed_Id]
GO
ALTER TABLE [dbo].[SubscriptionPostRead]  WITH CHECK ADD  CONSTRAINT [FK_SubscriptionPostRead_Post] FOREIGN KEY([PostId])
REFERENCES [dbo].[Post] ([Id])
GO
ALTER TABLE [dbo].[SubscriptionPostRead] CHECK CONSTRAINT [FK_SubscriptionPostRead_Post]
GO
ALTER TABLE [dbo].[SubscriptionPostRead]  WITH CHECK ADD  CONSTRAINT [FK_SubscriptionPostRead_Subscription] FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscription] ([Id])
GO
ALTER TABLE [dbo].[SubscriptionPostRead] CHECK CONSTRAINT [FK_SubscriptionPostRead_Subscription]
GO
USE [master]
GO
ALTER DATABASE [gdRead] SET  READ_WRITE 
GO
