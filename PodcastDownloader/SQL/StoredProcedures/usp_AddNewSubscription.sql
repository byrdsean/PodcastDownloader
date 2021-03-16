USE [PodcastDownloader]
GO

/****** Object:  StoredProcedure [dbo].[usp_AddNewSubscription]    Script Date: 3/15/2021 9:31:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		SEan Byrd
-- Create date: 3-1-2020
-- Description:	Add a new subscription to the "Podcasts" table
-- =============================================
CREATE PROCEDURE [dbo].[usp_AddNewSubscription]
	  @DisplayName nvarchar(1024)
	, @Url  nvarchar(2048)
	, @Abbreviation varchar(10)
	, @RSS_Url nvarchar(2048)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	/******
	Status Codes:
	--------------------
	Success = 0,
	Failure = 1,
	AlreadyExists = 2
	******/

	--Declare the status code to return
	declare @StatusCode int = 1

	--Check if the subscription already exists
	declare @ExistingCount int = 0
	select @ExistingCount = count(p.id)
	from Podcasts (nolock) as p
	where LOWER(p.RSS_Url) = LOWER(LTRIM(RTRIM(isnull(@RSS_Url, ''))))

	--If there is no existing data, insert
	if(0 < @ExistingCount) begin
		set @StatusCode = 2
	end
	else begin

		--Create transaction
		declare @TransactionName nvarchar(50) = 'AddSubscription'
		begin try
			begin transaction @TransactionName

			--Insert the data
			insert into [Podcasts]
			(
				[DisplayName]
				,[Url]
				,[Abbreviation]
				,[DateAdded]
				,[RSS_Url]
			)
			values
			(
				@DisplayName
				, @Url
				, @Abbreviation
				, GETDATE()
				, @RSS_Url
			)
			commit  transaction @TransactionName

			--Update the status code
			if (0 < @@IDENTITY) begin
				set @StatusCode = 0
			end
			else begin
				set @StatusCode = 1
			end
		end try
		begin catch
			rollback transaction @TransactionName
		end catch

	end

	--return the status code
	select @StatusCode as StatusCode

END

GO


