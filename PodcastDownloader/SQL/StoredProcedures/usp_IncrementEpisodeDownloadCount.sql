USE [PodcastDownloader]
GO

/****** Object:  StoredProcedure [dbo].[usp_IncrementEpisodeDownloadCount]    Script Date: 3/15/2021 9:31:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sean Byrd
-- Create date: 3-3-2020
-- Description:	Increment episode download count
-- =============================================
CREATE PROCEDURE [dbo].[usp_IncrementEpisodeDownloadCount]
	@EpisodeID integer
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	update [Episodes] 
	set   download_count = download_count + 1
		, LastDownloadDate = GETDATE()
	where id = @EpisodeID

END

GO


