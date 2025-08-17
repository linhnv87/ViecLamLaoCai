USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[ReportDocumentApproval]    Script Date: 3/21/2025 8:45:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ReportDocumentApproval]
	@Keyword nvarchar(250) = null,
	@FromDate varchar(10) = null,
	@ToDate varchar(10) = null,
	@PageNumber int,
	@PageSize int
AS
BEGIN
	declare @rawUserApproval table (
		UserId [uniqueidentifier],
		TotalOfDocID int,
		Release int,
		NoRelease int
	)
	declare @finalData table (
		UserFullName nvarchar(255),
		DepartmentId int, 
		DepartmentName nvarchar(255),
		UnitId int null,
		UnitName nvarchar(255),
		UserId [uniqueidentifier],
		TotalOfDocID int,
		Release int,
		NoRelease int
	)

	;with CTE_RawUserApproval as (
		select dr.DocID ID, dr.UserId,
				sum(
					case when ts.StatusCode = 'ban-hanh' then 1
					else 0
					end
				) Release,
				sum(
					case when ts.StatusCode = 'khong-ban-hanh' then 1
					else 0
					end
				) NoRelease
		from TblDocumentReview dr
		join TblDocumentHistories dh on dr.DocumentHistoryId = dh.Id
		join TblStatuses ts on dh.DocumentStatus = ts.ID
		join AppUsers au on dr.UserId = au.UserId
		where  (@FromDate is null or dr.Created >= @FromDate)
			and (@ToDate is null or cast(dr.Created as date) <= @ToDate)
			and ts.StatusCode in ('ban-hanh', 'khong-ban-hanh')
			and (@Keyword is null or au.UserFullName like '%' + @Keyword + '%')
		group by dr.UserId, DocID
	)

	insert into @rawUserApproval
	select UserId, count(ID) TotalOfDocId, sum(Release), sum(NoRelease)
	from CTE_RawUserApproval
	group by UserId

	insert into @finalData
	select u.UserFullName, u.DepartmentId, d.DepartmentName, u.UnitId, un.Name, [raw].*
	from @rawUserApproval [raw]
	join AppUsers u on [raw].UserId = u.UserId
	left join TblDeparments d on u.DepartmentId = d.Id
	left join TblUnits un on u.UnitId = un.Id

	-- return result
	select ROW_NUMBER() over(order by UserFullName) [Index], *
	from @finalData
	order by UserFullName
	OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

	select count(1) TotalRecords
	from @finalData

	select [raw].UserId, r.*
	from @rawUserApproval [raw]
	join AppUsersInRoles uir on [raw].UserId = uir.UserId
	join AppRoles r on uir.RoleId = r.RoleId
END
GO


