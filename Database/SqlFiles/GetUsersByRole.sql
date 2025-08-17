USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[GetUsersByRole]    Script Date: 12/30/2024 4:20:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetUsersByRole]
	@RoleName nvarchar(250)
AS
BEGIN
	select u.*
	from AppUsers u
	join AppUsersInRoles ur on u.UserId = ur.UserId
	join AppRoles r on ur.RoleId = r.RoleId
	where r.RoleName = @RoleName
	order by u.UserName
	OFFSET 0 ROWS FETCH NEXT 200 ROWS ONLY;
END
GO


