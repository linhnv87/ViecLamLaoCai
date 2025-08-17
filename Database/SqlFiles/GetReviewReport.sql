USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[GetReviewReport]    Script Date: 3/13/2025 11:44:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetReviewReport]
	@Keyword nvarchar(250) = null,
	@FromDate nvarchar(250) = null,
	@ToDate nvarchar(250) = null,
	@ReviewResult int = null,
	@SubmitCount int = null,
	@PageNumber int,
	@PageSize int,
	@SortColumn varchar(50) = null,
	@SortOrder varchar(4) = null
AS
BEGIN
	declare @ReviewUser table (
		UserId uniqueidentifier,
		NoReview int,
		DisAgree int,
		Agreed int,
		Other int
	)
	declare @FinalData table (
		UserId uniqueidentifier,
		NoReview int,
		DisAgree int,
		Agreed int,
		Other int,
		UserFullName nvarchar(250),
		PhoneNumber nvarchar(250)
	)
	declare @PaginationData table (
		UserId uniqueidentifier,
		NoReview int,
		DisAgree int,
		Agreed int,
		Other int,
		UserFullName nvarchar(250),
		PhoneNumber nvarchar(250)
	)

	insert into @ReviewUser
	select r.UserId, sum(iif(r.ReviewResult is null, 1, 0)), sum(iif(r.ReviewResult = 0, 1, 0)), sum(iif(r.ReviewResult = 1, 1, 0)), sum(iif(r.ReviewResult = 2, 1, 0))
	from TblDocumentReview r
	join AppUsers u on r.UserId = u.UserId
	where (@FromDate is null or r.Created >= @FromDate)
		and (@ToDate is null or r.Created < @ToDate)
		and (@Keyword is null or u.UserFullName like '%' + @Keyword + '%')
		and (@ReviewResult is null or r.ReviewResult = @ReviewResult)
		and (@SubmitCount is null or r.SubmitCount = @SubmitCount)
	group by r.UserId

	insert into @FinalData
	select ru.*, u.UserFullName, u.PhoneNumber
	from @ReviewUser ru
	join AppUsers u on ru.UserId = u.UserId

	insert into @PaginationData
	select *
	from @FinalData
	order by
		case
			when @SortColumn = 'userId' and @SortOrder = 'asc' then UserId
			when @SortColumn = 'userFullName' and @SortOrder = 'asc' then UserId
			when @SortColumn = 'phoneNumber' and @SortOrder = 'asc' then UserId
			else null
		end asc,
		case
			when @SortColumn = 'userId' and @SortOrder = 'desc' then UserId
			when @SortColumn = 'userFullName' and @SortOrder = 'desc' then UserId
			when @SortColumn = 'phoneNumber' and @SortOrder = 'desc' then UserId
			else null
		end desc
	OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

	select *
	from @PaginationData

	select count(1) TotalRecords from @FinalData

	select u.UserId, r.*
	from @PaginationData u
	join AppUsersInRoles uir on u.UserId = uir.UserId
	join AppRoles r on uir.RoleId = r.RoleId
END
GO


