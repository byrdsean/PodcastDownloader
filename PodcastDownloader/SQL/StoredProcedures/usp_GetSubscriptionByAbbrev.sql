USE [PodcastDownloader]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetSubscriptionByAbbrev]    Script Date: 3/15/2021 9:31:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sean Byrd
-- Create date: 3-1-2020
-- Description:	Get subscription data by abbreviation
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetSubscriptionByAbbrev]
	@Abbreviation varchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT [DisplayName]
		  ,[Url]
		  ,[Abbreviation]
		  ,[DateAdded]
		  ,[RSS_Url]
	FROM [Podcasts] (nolock) as p
	where LTRIM(RTRIM(isnull(@Abbreviation, ''))) <> ''
		and p.Abbreviation = @Abbreviation
END

GO


