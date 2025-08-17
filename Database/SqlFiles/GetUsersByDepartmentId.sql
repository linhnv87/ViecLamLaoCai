USE phab67cc_qltt
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
CREATE OR ALTER PROCEDURE dbo.GetUsersByDepartmentId 
	@DepartmentId int
AS
BEGIN
	declare @users table (
		[UserId] [uniqueidentifier],
		[UserName] [nvarchar](250),
		[UserFullName] [nvarchar](250),
		[HashedPassword] [nvarchar](250),
		[PasswordSalt] [nvarchar](250),
		[Email] [nvarchar](250),
		[PhoneNumber] [nvarchar](50),
		[CreateDate] [datetime2](7),
		[LastLoginDate] [datetime2](7),
		[IsApproved] [bit],
		[IsLockedout] [bit],
		[FieldOfficer] [nvarchar](50),
		[DepartmentId] [int],
		[Unit] [int] NULL
	)

	insert into @users
	select au.*
	from AppUsers au
	where au.DepartmentId = @DepartmentId

	select *
	from @users

	select uir.*
	from @users au
	join AppUsersInRoles uir on au.UserId = uir.UserId
END
GO
