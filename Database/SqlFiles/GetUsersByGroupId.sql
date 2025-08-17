USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[GetUsersByGroupId]    Script Date: 3/16/2025 7:36:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[GetUsersByGroupId]
	@GroupId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @usersInGroup table (
		Id INT,
		UserId UNIQUEIDENTIFIER
	)

	DECLARE @users TABLE (
		[UserId] UNIQUEIDENTIFIER,
		[UserName] NVARCHAR(250),
		[UserFullName] NVARCHAR(250),
		[HashedPassword] NVARCHAR(250),
		[PasswordSalt] NVARCHAR(250),
		[Email] NVARCHAR(250),
		[PhoneNumber] NVARCHAR(50),
		[CreateDate] DATETIME2(7),
		[LastLoginDate] DATETIME2(7),
		[IsApproved] BIT,
		[IsLockedout] BIT,
		[FieldOfficer] NVARCHAR(50),
		[DepartmentId] INT,
		[UnitId] [int] NULL
	);

	insert into @usersInGroup
	select distinct g.Id, gd.UserId
	from TblGroups g
	join TblGroupDetails gd on g.Id = gd.GroupId
	where g.Id = @GroupId
		and g.IsActive = 1


	INSERT INTO @users
	SELECT au.*
	FROM @usersInGroup u
	join AppUsers au on u.UserId = au.UserId

	SELECT *
	FROM @users;

	SELECT au.UserId, ar.*
	FROM @users au
	JOIN AppUsersInRoles uir ON au.UserId = uir.UserId
	JOIN AppRoles ar ON uir.RoleId = ar.RoleId;
	END
GO


