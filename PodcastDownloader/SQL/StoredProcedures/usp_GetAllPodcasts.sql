USE [PodcastDownloader]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetAllPodcasts]    Script Date: 3/15/2021 9:31:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sean Byrd
-- Create date: 3-1-2020
-- Description:	Get list of all podcasts
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetAllPodcasts]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [DisplayName] as Display_Name
		  ,[Url]
		  ,[Abbreviation]
		  ,[DateAdded]
		  ,[RSS_Url]
	  FROM [Podcasts](nolock)
	  order by [DateAdded] desc

END

GO


