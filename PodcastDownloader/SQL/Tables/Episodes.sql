USE [PodcastDownloader]
GO

/****** Object:  Table [dbo].[Episodes]    Script Date: 3/15/2021 9:10:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Episodes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[podcast_id] [bigint] NOT NULL,
	[title] [nvarchar](100) NOT NULL,
	[description] [nvarchar](max) NULL,
	[url] [nvarchar](2048) NOT NULL,
	[publication_date] [datetime] NULL,
	[download_count] [bigint] NOT NULL,
	[uniqueID] [nvarchar](1024) NULL,
	[LastDownloadDate] [datetime] NULL,
 CONSTRAINT [PK_episodes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


