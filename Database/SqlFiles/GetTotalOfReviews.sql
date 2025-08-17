USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[GetTotalOfReviews]    Script Date: 12/26/2024 7:54:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[GetTotalOfReviews]
	@UserId uniqueidentifier
AS
BEGIN
	declare @IdProcess table (
		Id int
	)
	declare @current_reviews table (
		ProcessStatusId int,
		ReviewDetailId int
	)
	declare @grouped_process_id table (
		Id int,
		TotalOfReviews int
	)
	declare @final_data table (
		[Id] [int],
		[Name] [nvarchar](100),
		[Description] [nvarchar](200),
		[Order] [int],
		TotalOfReviews int
	)

	insert into @IdProcess
	select Id
	from TblStatuses

	insert into @current_reviews
	select dh.DocumentStatus, dr.Id
	from TblDocuments d
	join TblDocumentHistories dh on d.CurrentDocumentHistoricalId = dh.Id
	join TblDocumentReview dr on dh.Id = dr.DocumentHistoryId
	where dr.UserId = @UserId

	insert into @grouped_process_id
	select process.Id, count(curr_review.ReviewDetailId)
	from @IdProcess process
	left join @current_reviews curr_review on process.Id = curr_review.ProcessStatusId
	group by process.Id

	insert into @final_data
	select process.Id, process.StatusCode, process.Title, process.[Order], id.TotalOfReviews
	from @grouped_process_id id
	join TblStatuses process on id.Id = process.Id

	select * from @final_data
END
GO


