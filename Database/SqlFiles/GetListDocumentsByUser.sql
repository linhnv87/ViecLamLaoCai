USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[GetListDocumentsByUser]    Script Date: 3/21/2025 8:51:36 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetListDocumentsByUser]
	@UserId nvarchar(250),
	@Type varchar(20)
AS
BEGIN
	if @Type = 'xin-y-kien'
	begin
		;with CTE_UserReview as (
			select dr.DocID, count(dr.ID) TotalOfReview
			from TblDocumentHistories dh
			join TblDocumentReview dr on dh.Id = dr.DocumentHistoryId
			join TblStatuses s on dh.DocumentStatus = s.ID
			where dr.UserId != dr.CreatedBy
				and dr.UserId = @UserId
				and (s.StatusCode = 'xin-y-kien' or s.StatusCode = 'xin-y-kien-lai')
			group by dr.DocID
		)

		select d.ID Id, d.Title, d.Note, d.StatusCode, d.AssigneeID, ur.TotalOfReview
		from CTE_UserReview ur
		join TblDocuments d on ur.DocID = d.ID
	end
	else if @Type = 'to-trinh'
	begin
		;with CTE_UserReview as (
			select dr.DocID, count(dr.ID) TotalOfReview
			from TblDocumentHistories dh
			join TblDocumentReview dr on dh.Id = dr.DocumentHistoryId
			join TblStatuses s on dh.DocumentStatus = s.ID
			where dr.UserId = @UserId
				and (s.StatusCode = 'ban-hanh' or s.StatusCode = 'khong-ban-hanh')
			group by dr.DocID
		)

		select d.ID Id, d.Title, d.Note, d.StatusCode, d.AssigneeID, ur.TotalOfReview
		from CTE_UserReview ur
		join TblDocuments d on ur.DocID = d.ID
	end
END
GO


