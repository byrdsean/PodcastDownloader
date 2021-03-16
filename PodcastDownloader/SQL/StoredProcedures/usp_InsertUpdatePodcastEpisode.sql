USE [PodcastDownloader]
GO

/****** Object:  StoredProcedure [dbo].[usp_InsertUpdatePodcastEpisode]    Script Date: 3/15/2021 9:31:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sean Byrd
-- Create date: 3-1-2020
-- Description:	Insert/Update Episode data for a podcast
-- =============================================
CREATE PROCEDURE [dbo].[usp_InsertUpdatePodcastEpisode]
		@title nvarchar(100)
	,	@description nvarchar(max)
	,	@url nvarchar(2048)
	,	@publication_date datetime = null
	,	@uniqueID nvarchar(1024)
	,	@abbreviation varchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Declare a true/false status flag
	declare @Status bit = 0

	--Get the podcast ID for the abbreviation
	declare @PodcastID integer = 0
	select top 1 @PodcastID = p.id
	from Podcasts(nolock) as p
	where p.Abbreviation = @abbreviation

	--If the ID is not a positive integer, then there is no corresponding podcast for the episode list
	if(0 < @PodcastID) begin
		--Check if the episode data is already present
		--If yes, update the data
		--If no, add the data
		declare @ExistsCount int = 0
		select @ExistsCount = count(*)
		from Episodes(nolock) as e
		where e.podcast_id = @PodcastID
			and e.uniqueID = @uniqueID

		--set the data
		declare @TransactionName varchar(50) = 'UpdateEpisodeList'
		begin try
			begin transaction @TransactionName

			if(@ExistsCount = 1) begin
				--If yes, update the data
				update Episodes
				set	 Episodes.[title] = @title
					,Episodes.[description] = @description
					,Episodes.[url] = @url
					,Episodes.[publication_date] = @publication_date
				where Episodes.podcast_id = @PodcastID
					and Episodes.uniqueID = @uniqueID

				--Commit the transaction
				commit transaction @TransactionName
				set @Status = 1
			end
			else begin
				--If no, add the data
				insert into Episodes
				(
					 [podcast_id]
					,[title]
					,[description]
					,[url]
					,[publication_date]
					,[download_count]
					,[uniqueID]
				)
				values
				(
					  @PodcastID
					, @title
					, @description
					, @url
					, @publication_date
					, 0
					, @uniqueID
				)

				--Commit and check that the data was inserted
				commit transaction @TransactionName
				if(0 < @@IDENTITY) begin
					set @Status = 1
				end
				else begin
					set @Status = 0
				end

			end

		end try
		begin catch
			rollback transaction @TransactionName
			set @Status = 0
		end catch

	end

	--Return the success flag
	select @Status as UpdateSuccess

END

GO


