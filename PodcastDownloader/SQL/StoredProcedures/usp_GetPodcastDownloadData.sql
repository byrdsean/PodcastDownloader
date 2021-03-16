USE [PodcastDownloader]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetPodcastDownloadData]    Script Date: 3/15/2021 9:31:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sean Byrd
-- Create date: 3-3-2020
-- Description:	Returns the podcast data for download
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetPodcastDownloadData]
		@Abbreviation varchar(10)
	,	@MaxReturn bigint
	,	@LatestEpisodes bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--validate the counts
	if(@MaxReturn <= @LatestEpisodes OR @MaxReturn = 0) begin
		set @MaxReturn = 40
		set @LatestEpisodes = 25
	end

	--get the max download count for this abbreviation
	declare @MaxDownloadCount bigint = 0
	SELECT @MaxDownloadCount = MAX(e.download_count)
	FROM [Podcasts](nolock) as p
		inner join Episodes(nolock) as e
		on e.podcast_id = p.id
	where p.Abbreviation = @Abbreviation

	--Get the latest episodes that are less than the max download count
	SELECT top(@LatestEpisodes)
			e.id
		,	e.title
		,	e.url
		,	e.publication_date
	into #RecentPods
	FROM [Podcasts](nolock) as p
		inner join Episodes(nolock) as e
		on e.podcast_id = p.id
	where p.Abbreviation = @Abbreviation
		--and e.download_count <= @MaxDownloadCount
		and 1 = 
		(
			case
				when @MaxDownloadCount = 0
				then (case when e.download_count <= @MaxDownloadCount then 1 else 0 end)
				else (case when e.download_count < @MaxDownloadCount then 1 else 0 end)
			end
		)
	order by e.publication_date desc

	--Get a random mix of episodes to fill the remaining count
	insert into #RecentPods(id, title, url, publication_date)
	SELECT top(@MaxReturn - (select count(*) from #RecentPods))
			e.id
		,	e.title
		,	e.url
		,	e.publication_date
	FROM [Podcasts](nolock) as p
		inner join Episodes(nolock) as e
		on e.podcast_id = p.id
		left join #RecentPods as RecentPods
		on RecentPods.id = e.id
	where p.Abbreviation = @Abbreviation
		and RecentPods.id is null
		and 1 = 
		(
			case
				when @MaxDownloadCount = 0
				then (case when e.download_count <= @MaxDownloadCount then 1 else 0 end)
				else (case when e.download_count < @MaxDownloadCount then 1 else 0 end)
			end
		)
	order by NEWID() asc

	--Return the podcast selected
	select r.id
		, r.title
		, r.url
		, r.publication_date
	from #RecentPods as r
	order by publication_date desc

	--Make sure to drop the temp tables
	drop table #RecentPods

END

GO


