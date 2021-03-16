USE [PodcastDownloader]
GO

/****** Object:  Table [dbo].[Podcasts]    Script Date: 3/15/2021 9:10:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Podcasts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](1024) NOT NULL,
	[Url] [nvarchar](2048) NOT NULL,
	[Abbreviation] [varchar](10) NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[RSS_Url] [nvarchar](2048) NOT NULL,
 CONSTRAINT [PK_Podcasts] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


