USE [phab67cc_qltt]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE dbo.GetAllAppUsers
AS
BEGIN
	declare @user TABLE (
		[UserId] [uniqueidentifier] NOT NULL,
		[UserName] [nvarchar](250) NULL,
		[UserFullName] [nvarchar](250) NULL,
		[HashedPassword] [nvarchar](250) NULL,
		[PasswordSalt] [nvarchar](250) NULL,
		[Email] [nvarchar](250) NULL,
		[PhoneNumber] [nvarchar](50) NULL,
		[CreateDate] [datetime2](7) NULL,
		[LastLoginDate] [datetime2](7) NULL,
		[IsApproved] [bit] NULL,
		[IsLockedout] [bit] NULL,
		[FieldOfficer] [nvarchar](50) NULL,
		[DepartmentId] [int] NULL,
		[Unit] [int] NULL
	)

	declare @roles table (
		[UserId] [uniqueidentifier] NOT NULL,
		[RoleId] [uniqueidentifier] NOT NULL,
		[RoleName] [nvarchar](250) NULL,
		[Description] [nvarchar](250) NULL,
		[Active] [bit] NULL,
		[Deleted] [bit] NULL
	)

	insert into @user
	select *
	from AppUsers
	where UserName != 'superadmin'

	insert into @roles
	select au.UserId, r.*
	from @user au
	join AppUsersInRoles uir on au.UserId = uir.UserId
	join AppRoles r on uir.RoleId = r.RoleId

	select *
	from @user	

	select *
	from @roles
END
GO
