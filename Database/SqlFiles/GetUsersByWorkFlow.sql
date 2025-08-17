USE [phab67cc_qltt]
GO

/****** Object:  StoredProcedure [dbo].[GetUsersByWorkFlow]    Script Date: 2/17/2025 9:23:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetUsersByWorkFlow]
	@UserId nvarchar(max),
	@StatusId int,
	@State varchar(100)
AS
BEGIN
	declare @workFlow table (
		[Id] [int],
		[Name] [nvarchar](max),
		[PrevWorkflowId] [int],
		[NextWorkflowId] [int],
		[DefaultUserId] [nvarchar](max),
		[IsSign] [bit] NULL,
		[Description] [nvarchar](max),
		[StatusId] [int],
		[UserId] [nvarchar](max),
		[State] [varchar](100)
	)

	declare @assignee table (
		UserId uniqueidentifier,
		UserFullName nvarchar(250),
		IsDefault bit
	)

	declare @roles table (
		UserId uniqueidentifier,
		RoleId uniqueidentifier,
		RoleName nvarchar(250),
		Description nvarchar(250)
	)

	insert into @workFlow
	select wf.*
	from CfgWorkFlow wf
	where wf.UserId = @UserId and wf.StatusId = @StatusId and wf.State = @State

	-- default user
	insert into @assignee
	select u.UserId, u.UserFullName, 1
	from @workFlow wf
	join AppUsers u on wf.DefaultUserId = u.UserId

	-- roles
	insert into @assignee
	select u.UserId, u.UserFullName, wfg.IsDefault
	from @workFlow wf
	join CfgWorkFlowGroup wfg on wf.Id = wfg.WorkflowId
	join AppUsersInRoles uir on wfg.RoleId = uir.RoleId
	join AppUsers u on uir.UserId = u.UserId

	-- default user in role
	insert into @assignee
	select u.UserId, u.UserFullName, wfg.IsDefault
	from @workFlow wf
	join CfgWorkFlowGroup wfg on wf.Id = wfg.WorkflowId
	join AppUsers u on wfg.DefaultUserId = u.UserId

	-- users
	insert into @assignee
	select distinct u.UserId, u.UserFullName, wfu.IsDefault
	from @workFlow wf
	join CfgWorkFlowUser wfu on wf.Id = wfu.WorkflowId
	join AppUsers u on wfu.UserId = u.UserId

	-- roles
	insert into @roles
	select a.UserId, r.RoleId, r.RoleName, r.Description
	from @assignee a
	join AppUsersInRoles uir on a.UserId = uir.UserId
	join AppRoles r on uir.RoleId = r.RoleId

	-- return workflow including users
	select *
	from @workFlow

	select *
	from @assignee
	order by IsDefault desc

	select *
	from @roles
END
GO


