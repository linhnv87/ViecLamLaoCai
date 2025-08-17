USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[ReportApprovalByUser]    Script Date: 3/20/2025 10:59:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[ReportApprovalByUser]
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
		NoReview int,
		DisAgreed int,
		Agreed int,
		Other int
	)
	declare @finalData table (
		UserFullName nvarchar(250),
		DepartmentId int, 
		DepartmentName nvarchar(255),
		UnitId int null,
		UnitName nvarchar(255),
		UserId [uniqueidentifier],
		TotalOfDocID int,
		NoReview int,
		DisAgreed int,
		Agreed int,
		Other int
	)

	;WITH CTE_RawUserApproval AS (
    SELECT dr.UserId, dr.DocID,
        SUM(
            CASE 
                WHEN ReviewResult = 1 AND LTRIM(RTRIM(ISNULL(dr.Comment, ''))) = N'Hệ thống tự động gán Đồng ý' 
                THEN 1 ELSE 0 
            END
        ) AS NoReview,
        SUM(
            CASE 
                WHEN ReviewResult = 0 THEN 1 
                ELSE 0 
            END
        ) AS DisAgreed,
        SUM(
            CASE 
                WHEN ReviewResult = 1 AND LTRIM(RTRIM(ISNULL(dr.Comment, ''))) != N'Hệ thống tự động gán Đồng ý' 
                THEN 1 ELSE 0 
            END
        ) AS Agreed,
        SUM(
            CASE 
                WHEN ReviewResult = 2 THEN 1 
                ELSE 0 
            END
        ) AS Other
    FROM TblDocumentReview dr
    JOIN TblDocumentHistories dh ON dr.DocumentHistoryId = dh.Id
    JOIN TblStatuses ts ON dh.DocumentStatus = ts.ID
    JOIN AppUsers au ON dr.UserId = au.UserId
    WHERE dr.UserId != dr.CreatedBy
        AND (@FromDate IS NULL OR dr.Created >= @FromDate)
        AND (@ToDate IS NULL OR CAST(dr.Created AS DATE) <= @ToDate)
        AND (@Keyword IS NULL OR au.UserFullName LIKE '%' + @Keyword + '%')
        AND dr.SubmitCount > 0
    GROUP BY dr.UserId, dr.DocID, dr.SubmitCount
	)


	insert into @rawUserApproval
	select UserId, count(DocID), sum(NoReview), sum(DisAgreed), sum(Agreed), sum(Other)
	from CTE_RawUserApproval
	group by UserId

	insert into @finalData
	select u.UserFullName, u.DepartmentId, d.DepartmentName, u.UnitId, un.Name UnitName, [raw].*
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


