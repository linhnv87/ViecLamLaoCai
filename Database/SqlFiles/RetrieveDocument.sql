USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[RetrieveDocument]    Script Date: 2/17/2025 9:53:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[RetrieveDocument]
	@DocumentId int,
	@CurrentUserId uniqueidentifier,
	@Note nvarchar(250),
	@Comment nvarchar(250)
AS
BEGIN
	declare @viewedTable table (
		Id int,
		ReviewResult int null,
		UserId uniqueidentifier null
	)

	-- check TblDocumentReview table
	insert into @viewedTable
	select dr.Id, dr.ReviewResult, dr.UserId
	from TblDocuments d
	join TblDocumentHistories dh on d.CurrentDocumentHistoricalId = dh.Id
	join TblDocumentReview dr on dh.Id = dr.DocumentHistoryId
	where dr.Viewed != 0
		and d.ID = @DocumentId

	-- check document is viewd or not
	IF(EXISTS(SELECT 1 FROM @viewedTable))
	BEGIN
	  RAISERROR(N'Văn bản đã được xem.',16,10);
	END
	else
	begin
		declare @lastDocumentStatus int
		declare @lastIdentityId int

		-- if not viewed yet
		-- get previous status of document basing on history table
		select top 1 @lastDocumentStatus = DocumentStatus
		from (
			select top 2 Created, DocumentStatus
			from TblDocumentHistories dh
			where dh.DocumentId = @DocumentId
			order by Created desc
		) as latest
		order by Created asc

		-- after getting DocumentStatus then insert a new record to history table
		insert into TblDocumentHistories (DocumentId, DocumentTitle, Note, Created, CreatedBy, DocumentStatus, Comment)
		select ID, Title, @Note, getdate(), @CurrentUserId, @lastDocumentStatus, @Comment
		from TblDocuments
		where Id = @DocumentId

		SELECT @lastIdentityId = scope_identity()

		-- update current historical_id to table TblDocuments
		update TblDocuments
		set CurrentDocumentHistoricalId = @lastIdentityId
		where Id = @DocumentId

	end
END
GO


